using Scheduler.Application.Infrastructure.Data.PostgreSql;
using System;

namespace Scheduler.UnitTests.Application.Infrastructure.Data.PostgreSql
{
    [TestClass]
    public class PostgreSqlDataContextTests
    {
        [TestMethod]
        public void GetConnection_ShouldReturnNpgsqlConnection()
        {
            const string varName = "POSTGRE_SQL_CONNECTION_STRING";
            var original = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.Process);

            try
            {
                //Arrange
                Environment.SetEnvironmentVariable(varName, "Host=localhost;Username=postgres;Password=pass;Database=test;", EnvironmentVariableTarget.Process);
                var dataContext = new PostgreSqlDataContext();

                //Act
                var connection = dataContext.GetConnection();

                //Assert
                Assert.IsInstanceOfType(connection, typeof(Npgsql.NpgsqlConnection));
            }
            finally
            {
                Environment.SetEnvironmentVariable(varName, original, EnvironmentVariableTarget.Process);
            }
        }
    }
}
