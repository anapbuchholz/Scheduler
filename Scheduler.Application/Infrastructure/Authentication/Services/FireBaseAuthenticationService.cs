using FirebaseAdmin.Auth;
using Scheduler.Application.Infrastructure.Authentication.Services.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services
{
    internal sealed class FireBaseAuthenticationService(HttpClient httpClient) : IFireBaseAuthenticationService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly FirebaseAuth _firebaseAdmin = FirebaseAuth.DefaultInstance;

        public async Task<(bool IsAuthenticated, string? JwtToken)> LoginInFireBase(string email, string password)
        {
            var request = new
            {
                email,
                password,
                returnSecureToken = true
            };

            //TODO: INJETAR O HTTPcLIENT VIA DEPENDENCY INJECTION, PASSAR A BASE ADDRESS E A API KEY VIA CONFIGURAÇÃO.
            var response = await _httpClient.PostAsJsonAsync("", request);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
                return (true, responseData?.IdToken);
            }

            return (false, null);
        }

        public async Task<(string? ExternalId, bool RegisteredWithSuccess)> RegisterFireBaseUserAsync(string email, string password, string displayName)
        {
            try
            {
                var userArgs = new UserRecordArgs()
                {
                    Email = email,
                    Password = password,
                    DisplayName = displayName
                };

                var userRecord = await _firebaseAdmin.CreateUserAsync(userArgs);
                return (userRecord.Uid, true);
            }
            catch
            {
                return (null, false);
            }
        }

        public async Task UpdateFireBaseUser(UserRecordArgs userArgs)
        {
            await _firebaseAdmin.UpdateUserAsync(userArgs);
        }

        public async Task<bool> DeleteFireBaseUserAsync(string userEmail)
        {
            try
            {
                var userRecord = await _firebaseAdmin.GetUserByEmailAsync(userEmail);
                await _firebaseAdmin.DeleteUserAsync(userRecord.Uid);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserRecord> GetFireBaseUserByEmail(string userEmail)
        {
            return await _firebaseAdmin.GetUserByEmailAsync(userEmail);
        }
    }
}
