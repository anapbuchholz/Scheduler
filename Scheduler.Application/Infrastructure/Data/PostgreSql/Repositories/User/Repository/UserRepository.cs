using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
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

        public async Task UpdateUserAsync(Guid Id, UserEntity user)
        {
            var command = UserSqlConstants.UPDATE_USER_BY_ID;
            await _sqlHelper.ExecuteAsync(command, new
            {
                user.Name,
                user.DocumentNumber,
                user.PasswordHash,
                user.IsAdmin,
                Id
            });
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var command = UserSqlConstants.DELETE_USER_BY_ID;
            await _sqlHelper.ExecuteAsync(command, new { Id = id });
        }

        public Task<PaginatedQueryResult<UserEntity>> ListUsersAsync(string? name, string? email, string? documentNumber, bool? isAdmin, PaginationInput paginationParameters)
        {
            var whereClause = UserSqlConstants.ListUsersPaginationConstants.LIST_USERS_WHERE_STATEMENT;
            var parameters = new Dapper.DynamicParameters();

            if (!string.IsNullOrWhiteSpace(name))
            {
                whereClause += " AND LOWER(name) LIKE @Name";
                parameters.Add("Name", $"%{name.ToLower()}%");
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                whereClause += " AND LOWER(email) LIKE @Email";
                parameters.Add("Email", $"%{email.ToLower()}%");
            }
            if (!string.IsNullOrWhiteSpace(documentNumber))
            {
                whereClause += " AND tax_id = @DocumentNumber";
                parameters.Add("DocumentNumber", documentNumber);
            }
            if (isAdmin.HasValue)
            {
                whereClause += " AND is_admin = @IsAdmin";
                parameters.Add("IsAdmin", isAdmin.Value);
            }
            
            var userList = _sqlHelper.SelectPaginated<UserEntity>(
                paginationParameters,
                UserSqlConstants.ListUsersPaginationConstants.LIST_USERS_SELECT_STATEMENT,
                UserSqlConstants.ListUsersPaginationConstants.LIST_USERS_FROM_STATEMENT,
                whereClause,
                true,
                parameters);

            return userList;
        }
    }
}
