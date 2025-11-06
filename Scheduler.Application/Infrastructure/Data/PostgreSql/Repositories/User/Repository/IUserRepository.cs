using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository
{
    internal interface IUserRepository
    {
        Task RegisterUserAsync(UserEntity user);

        Task<UserEntity?> GetUserByIdAsync(Guid Id);

        Task<UserEntity?> GetUserByEmailAsync(string email);

        Task<UserEntity?> GetUserByDocumentNumberAsync(string documentNumber);
    }
}
