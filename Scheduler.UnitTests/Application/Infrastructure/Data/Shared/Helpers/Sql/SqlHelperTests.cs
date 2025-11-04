using Dapper;
using Microsoft.Data.Sqlite;
using Moq;
using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.Shared.Context;
using Scheduler.Application.Infrastructure.Data.Shared.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Infrastructure.Data.Shared.Helpers.Sql
{
    [TestClass]
    public class SqlHelperTests
    {
        private readonly Mock<IDataContext> _dataContextMock;
        private readonly SqlHelper _service;

        public SqlHelperTests()
        {
            _dataContextMock = new Mock<IDataContext>();

            // Registrar handler para Guid e Guid? — deve ser feito antes das chamadas Dapper
            var guidHandler = new GuidTypeHandlerForTests();
            SqlMapper.AddTypeHandler(typeof(Guid), guidHandler);
            SqlMapper.AddTypeHandler(typeof(Guid?), guidHandler);

            _service = new SqlHelper(_dataContextMock.Object);
        }

        private static string CreateInMemoryConnectionString() =>
            "Data Source=InMemoryTests;Mode=Memory;Cache=Shared";

        // Keeper: mantém o DB in-memory vivo enquanto aberto
        private static SqliteConnection CreateKeeper(string connectionString)
        {
            var keeper = new SqliteConnection(connectionString);
            keeper.Open();
            return keeper;
        }

        // Configura o mock para retornar uma NOVA conexão para cada chamada.
        // As conexões retornadas NÃO devem ser abertas aqui (o serviço / Dapper fará open quando necessário).
        private void SetupMockToReturnNewConnections(string connectionString)
        {
            _dataContextMock.Setup(x => x.GetConnection()).Returns(() =>
            {
                // retorna nova instância fechada; SqlService usará/abrirá e depois disporá
                return new SqliteConnection(connectionString);
            });
        }

        private static async Task PrepareSimpleTable(SqliteConnection conn, int rows = 5, bool? setAllIsActiveTo = null, List<Guid> idsToInsert = null)
        {
            await conn.ExecuteAsync("CREATE TABLE Items (Id TEXT PRIMARY KEY, Name TEXT, IsActive INTEGER);");
            if (idsToInsert != null)
            {
                var i = 0;
                foreach (var id in idsToInsert)
                {
                    var isActive = setAllIsActiveTo == null ? i % 2 == 0 ? 1 : 0 : setAllIsActiveTo == true ? 1 : 0;
                    await conn.ExecuteAsync("INSERT INTO Items (Id, Name, IsActive) VALUES (@Id, @Name, @IsActive);", new { Id = id.ToString(), Name = $"Item_{id}", IsActive = isActive });
                    i++;
                }

                return;
            }

            for (int i = 1; i <= rows; i++)
            {
                var isActive = setAllIsActiveTo == null ? i % 2 == 0 ? 1 : 0 : setAllIsActiveTo == true ? 1 : 0;
                await conn.ExecuteAsync("INSERT INTO Items (Id, Name, IsActive) VALUES (@Id, @Name, @IsActive);", new { Id = Guid.NewGuid(), Name = $"Item{i}", IsActive = isActive });
            }
        }

        [TestMethod]
        public async Task CountAsync_ReturnsCorrectCount()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            await PrepareSimpleTable(keeper, 3);

            var count = await _service.CountAsync("SELECT COUNT(*) FROM Items");
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public async Task ExecuteAsync_InsertsRow_ReturnsRowsAffected()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            // criar tabela com o keeper (mantém DB vivo)
            await keeper.ExecuteAsync("CREATE TABLE Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);");
            var initialCount = await keeper.QueryFirstAsync<int>("SELECT COUNT(*) FROM Items;");
            Assert.AreEqual(0, initialCount);

            var rows = await _service.ExecuteAsync("INSERT INTO Items (Name) VALUES (@Name);", new { Name = "New" });
            Assert.AreEqual(1, rows);

            var count = await keeper.QueryFirstAsync<int>("SELECT COUNT(*) FROM Items;");
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task SelectAsync_ReturnsEnumerable()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            await PrepareSimpleTable(keeper, 4);

            var results = (await _service.SelectAsync<Item>("SELECT Id, Name, IsActive FROM Items ORDER BY Id")).ToList();

            Assert.HasCount(4, results);
            Assert.IsTrue(results.All(r => r.Name.StartsWith("Item")));
        }

        [TestMethod]
        public async Task SelectSingleAsync_ReturnsSingle()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);
            var singleId = Guid.NewGuid();
            await PrepareSimpleTable(keeper, 2, null, [singleId]);

            var single = await _service.SelectFirstOrDefaultAsync<Item>($"SELECT Id, Name, IsActive FROM Items WHERE Id = '{singleId}'");

            Assert.IsNotNull(single);
            Assert.AreEqual($"Item_{singleId}", single.Name);
        }

        [TestMethod]
        public async Task ExecuteWithTransactionAsync_AllCommandsSucceed_CommitsAndReturnsTotal()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            // criação da tabela pelo keeper
            await keeper.ExecuteAsync("CREATE TABLE Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);");

            var commands = new Dictionary<string, object>()
            {
                { "INSERT INTO Items (Name) VALUES (@Name1);", new { Name1 = "A" } },
                { "INSERT INTO Items (Name) VALUES (@Name2);", new { Name2 = "B" } }
            };

            var (success, rows) = await _service.ExecuteWithTransactionAsync(commands);

            Assert.IsTrue(success);
            Assert.AreEqual(2, rows);

            var count = await keeper.QueryFirstAsync<int>("SELECT COUNT(*) FROM Items;");
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task ExecuteWithTransactionAsync_ZeroRowsAffected_RollsBackAndReturnsFalse()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            await keeper.ExecuteAsync("CREATE TABLE Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);");
            await keeper.ExecuteAsync("INSERT INTO Items (Name) VALUES (@Name);", new { Name = "Initial" });

            var commands = new Dictionary<string, object>()
            {
                { "UPDATE Items SET Name = @Name WHERE Id = 1;", new { Name = "Updated" } },
                { "UPDATE Items SET Name = @Name WHERE Id = -1;", new { Name = "NoOne" } }
            };

            var (success, rows) = await _service.ExecuteWithTransactionAsync(commands);

            Assert.IsFalse(success);
            Assert.AreEqual(0, rows);

            var name = await keeper.QueryFirstAsync<string>("SELECT Name FROM Items WHERE Id = 1;");
            Assert.AreEqual("Initial", name);
        }

        [TestMethod]
        public async Task ExecuteWithTransactionAsync_ExceptionThrown_RollsBackAndReturnsFalse()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            await keeper.ExecuteAsync("CREATE TABLE Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);");

            var commands = new Dictionary<string, object>()
            {
                { "INSERT INTO Items (Name) VALUES (@Name);", new { Name = "OK" } },
                { "INSERT INTO NonExisting (Col) VALUES (1);", null }
            };

            var (success, rows) = await _service.ExecuteWithTransactionAsync(commands);

            Assert.IsFalse(success);
            Assert.AreEqual(0, rows);

            var count = await keeper.QueryFirstAsync<int>("SELECT COUNT(*) FROM Items;");
            Assert.AreEqual(0, count); // rollback
        }

        [TestMethod]
        public async Task SelectPaginated_NoSearchParam_ReturnsCorrectPageAndTotal()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            await PrepareSimpleTable(keeper, 5);

            var request = PaginationInput.Create(2, 2, null); // page 2, size 2 -> items 3 and 4
            var select = "SELECT Id, Name, IsActive";
            var from = "FROM Items";
            var where = "";

            var result = await _service.SelectPaginated<Item>(request, select, from, where);

            Assert.AreEqual(5, result.TotalCount);
            Assert.AreEqual(2, result.Results.Count());
            var ids = result.Results.Select(r => r.Id).OrderBy(x => x).ToList();
        }

        [TestMethod]
        public async Task SelectPaginated_WithSearchParam_ReplacesPlaceholderAndFilters()
        {
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            await PrepareSimpleTable(keeper, 5);

            var request = PaginationInput.Create(1, 10, "Item3");
            var select = "SELECT Id, Name, IsActive";
            var from = "FROM Items";
            var where = "WHERE Name LIKE '%@search_param%'";

            var result = await _service.SelectPaginated<Item>(request, select, from, where);

            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Results.Count());
            Assert.IsTrue(result.Results.Any(r => r.Name == "Item3"));
        }

        [TestMethod]
        public async Task SelectPaginated_IndependentWhereStatementTrue_KeepsWhereEvenIfSearchNull()
        {
            var totalRows = 20;
            var pageSize = 10;
            var cs = CreateInMemoryConnectionString();
            using var keeper = CreateKeeper(cs);
            SetupMockToReturnNewConnections(cs);

            await PrepareSimpleTable(keeper, totalRows, true);

            var request = PaginationInput.Create(1, pageSize, null);
            var select = "SELECT Id, Name, IsActive";
            var from = "FROM Items";
            var where = "WHERE IsActive = 1";

            var result = await _service.SelectPaginated<Item>(request, select, from, where, independentWhereStatement: true);

            Assert.AreEqual(totalRows, result.TotalCount);
            var names = result.Results.Select(r => r.Name).ToList();
            Assert.IsTrue(names.Any(n => n.StartsWith("Item")));
            Assert.AreEqual(pageSize, result.Results.Count());
        }

        private class Item : BaseEntity
        {
            public string Name { get; set; } = string.Empty;
            public int IsActive { get; set; }
        }
    }
}