using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository
{
    internal sealed class UserRepository(ISqlHelper sqlhelper) : IUserRepository
    {
        private readonly ISqlHelper _sqlHelper = sqlhelper;

        public async Task<UserEntity?> GetUserByIdAsync(Guid Id)
        {
            var query = UserSqlConstants.SELECT_USER_BY_ID;
            return await _sqlHelper.SelectFirstOrDefaultAsync<UserEntity>(query, new { Id });
        }

        public async Task<UserEntity?> GetUserByDocumentNumberAsync(string documentNumber)
        {
            var query = UserSqlConstants.SELECT_USER_BY_DOCUMENT_NUMBER;
            return await _sqlHelper.SelectFirstOrDefaultAsync<UserEntity>(query, new { DocumentNumber = documentNumber });
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            var query = UserSqlConstants.SELECT_USER_BY_EMAIL;
            return await _sqlHelper.SelectFirstOrDefaultAsync<UserEntity>(query, new { Email = email });
        }

        public async Task RegisterUserAsync(UserEntity user)
        {
            var command = UserSqlConstants.INSERT_USER;
            await _sqlHelper.ExecuteAsync(command, user);
        }
    }
}
