using FirebaseAdmin.Auth;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces
{
    internal interface IFireBaseAdminProxy
    {
        Task<UserRecord> GetUserByEmailAsync(string userEmail);
        Task<UserRecord> CreateUserAsync(UserRecordArgs userArgs);
        Task<UserRecord> UpdateUserAsync(UserRecordArgs userArgs);
        Task DeleteUserAsync(string uid);
    }
}
