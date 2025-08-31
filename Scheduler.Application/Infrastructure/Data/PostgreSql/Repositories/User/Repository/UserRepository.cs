using Dapper;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Context;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository
{
    internal sealed class UserRepository(IDataContext context) : IUserRepository
    {
        private readonly IDataContext _context = context;

        public Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            var query = UserSqlConstants.SELECT_USER_BY_EMAIL;

            var connection = _context.GetConnection();
            return connection.QueryFirstOrDefaultAsync<UserEntity>(query, new { Email = email });
        }

        public Task RegisterUserAsync(UserEntity user)
        {
            var command = UserSqlConstants.INSERT_USER;

            var connection = _context.GetConnection();
            return connection.ExecuteAsync(command, user);
        }
    }
}
