using FirebaseAdmin.Auth;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces
{
    internal interface IFireBaseAuthenticationService
    {
        Task<(bool IsAuthenticated, string? JwtToken)> LoginInFireBaseAsync(string email, string password);

        Task<(string? ExternalId, bool RegisteredWithSuccess)> RegisterFireBaseUserAsync(string email, string password, string name, bool isAdmin);

        Task<(string? ExternalId, bool RegisteredWithSuccess)> UpdateFireBaseUserAsync(string uid, string email, string password, string name, bool isAdmin);

        Task<UserRecord> GetFireBaseUserByEmailAsync(string userEmail);

        Task<bool> DeleteFireBaseUserAsync(string userEmail);
    }
}
