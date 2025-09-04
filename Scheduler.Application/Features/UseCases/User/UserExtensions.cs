using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.UseCases.User.Login;
using Scheduler.Application.Features.UseCases.User.RegisterUser;

namespace Scheduler.Application.Features.UseCases.User
{
    internal static class UserExtensions
    {
        public static IServiceCollection AddUserFeatures(this IServiceCollection services)
        {
            services.AddRegisterUser();
            services.AddLogin();
            return services;
        }
    }
}
