using Npgsql;
using Scheduler.Application.Infrastructure.Configuration;
using Scheduler.Application.Infrastructure.Data.Shared.Context;
using System.Data;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql
{
    internal sealed class PostgreSqlDataContext : IDataContext
    {
        private const string CONNECTION_STRING_VARIABLE_NAME = "POSTGRE_SQL_CONNECTION_STRING";
        private readonly string _connectionString;

        public PostgreSqlDataContext()
        {
            _connectionString = EnvironmentVariableHandler.GetEnvironmentVariable(CONNECTION_STRING_VARIABLE_NAME);
        }

        public IDbConnection GetConnection() => new NpgsqlConnection(_connectionString);
    }
}
