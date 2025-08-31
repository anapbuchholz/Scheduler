using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication
{
    internal interface IAuthenticationService
    {
        Task<string> RegisterAsync(string email, string password, string displayName);
    }
}
