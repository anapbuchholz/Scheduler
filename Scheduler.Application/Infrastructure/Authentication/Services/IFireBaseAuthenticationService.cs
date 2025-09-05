using FirebaseAdmin.Auth;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services
{
    internal interface IFireBaseAuthenticationService
    {
        Task<(bool IsAuthenticated, string? JwtToken)> LoginInFireBase(string email, string password);

        Task<(string? ExternalId, bool RegisteredWithSuccess)> RegisterFireBaseUserAsync(string email, string password, string displayName);

        Task<bool> DeleteFireBaseUserAsync(string userEmail);

        Task<UserRecord> GetFireBaseUserByEmail(string userEmail);

        Task UpdateFireBaseUser(UserRecordArgs userArgs);
    }
}
