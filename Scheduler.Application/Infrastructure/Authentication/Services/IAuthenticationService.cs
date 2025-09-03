using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Services
{
    internal interface IAuthenticationService
    {
        Task<(string? ExternalId, bool RegisteredWithSuccess)> RegisterUserAsync(string email, string password, string displayName);
    }
}
