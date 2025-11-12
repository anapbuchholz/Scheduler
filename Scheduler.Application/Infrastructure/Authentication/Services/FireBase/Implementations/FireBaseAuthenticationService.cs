using FirebaseAdmin.Auth;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using Scheduler.Application.Infrastructure.Authentication.Services.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Implementations
{
    internal sealed class FireBaseAuthenticationService(HttpClient httpClient, IFireBaseAdminProxy fireBaseAdminProxy) : IFireBaseAuthenticationService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IFireBaseAdminProxy _firebaseAdminProxy = fireBaseAdminProxy;

        public async Task<(bool IsAuthenticated, string? JwtToken)> LoginInFireBaseAsync(string email, string password)
        {
            var request = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync("", request);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
                return (true, responseData?.IdToken);
            }

            return (false, null);
        }

        public async Task<(string? ExternalId, bool RegisteredWithSuccess)> RegisterFireBaseUserAsync(string email, string password, string name, bool isAdmin)
        {
            try
            {
                string displayName = DefineUserDisplayName(name, isAdmin);
                var userArgs = new UserRecordArgs()
                {
                    Email = email,
                    Password = password,
                    DisplayName = displayName
                };

                var userRecord = await _firebaseAdminProxy.CreateUserAsync(userArgs);
                return (userRecord.Uid, true);
            }
            catch
            {
                return (null, false);
            }
        }

        public async Task<(string? ExternalId, bool RegisteredWithSuccess)> UpdateFireBaseUserAsync(string uid, string email, string password, string name, bool isAdmin)
        {
            try
            {
                string displayName = DefineUserDisplayName(name, isAdmin);
                var userArgs = new UserRecordArgs()
                {
                    Uid = uid,
                    Email = email,
                    Password = password,
                    DisplayName = displayName
                };

                var userRecord = await _firebaseAdminProxy.UpdateUserAsync(userArgs);
                return (userRecord.Uid, true);
            }
            catch
            {
                return (null, false);
            }
        }

        public async Task<bool> DeleteFireBaseUserAsync(string userEmail)
        {
            try
            {
                var userRecord = await _firebaseAdminProxy.GetUserByEmailAsync(userEmail);
                await _firebaseAdminProxy.DeleteUserAsync(userRecord.Uid);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserRecord> GetFireBaseUserByEmailAsync(string userEmail)
        {
            return await _firebaseAdminProxy.GetUserByEmailAsync(userEmail);
        }

        #region Private methods
        private static string DefineUserDisplayName(string name, bool isAdmin)
        {
            var userPermission = isAdmin ? "1" : "0";
            var displayName = $"{name}-{userPermission}";
            return displayName;
        }
        #endregion
    }
}
