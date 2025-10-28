using FirebaseAdmin.Auth;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Implementations
{
    [ExcludeFromCodeCoverage]
    internal sealed class FireBaseAdminProxy : IFireBaseAdminProxy
    {
        private readonly FirebaseAuth _firebaseAdmin = FirebaseAuth.DefaultInstance;

        public async Task<UserRecord> CreateUserAsync(UserRecordArgs userArgs)
        {
            return await _firebaseAdmin.CreateUserAsync(userArgs);
        }

        public async Task DeleteUserAsync(string uid)
        {
            await _firebaseAdmin.DeleteUserAsync(uid);
        }

        public async Task<UserRecord> GetUserByEmailAsync(string userEmail)
        {
            return await _firebaseAdmin.GetUserByEmailAsync(userEmail);
        }

        public async Task<UserRecord> UpdateUserAsync(UserRecordArgs userArgs)
        {
            return await _firebaseAdmin.UpdateUserAsync(userArgs);
        }
    }
}
