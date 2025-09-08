using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository
{
    internal interface IUserRepository
    {
        Task RegisterUserAsync(UserEntity user);

        Task<UserEntity?> GetUserByEmailAsync(string email);
    }
}
