using FirebaseAdmin.Auth;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        public async Task<(string? ExternalId, bool RegisteredWithSuccess)> RegisterUserAsync(string email, string password, string displayName)
        {
            try
            {
                var userArgs = new UserRecordArgs()
                {
                    Email = email,
                    Password = password,
                    DisplayName = displayName
                };

                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
                return (userRecord.Uid, true);
            }
            catch
            {
                return (null, false);
            }
        }
    }
}
