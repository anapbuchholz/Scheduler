using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.UseCases.User.GetUser;
using Scheduler.Application.Features.UseCases.User.Login;
using Scheduler.Application.Features.UseCases.User.RegisterUser;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User
{
    [ExcludeFromCodeCoverage]
    internal static class UserExtensions
    {
        public static IServiceCollection AddUserFeatures(this IServiceCollection services)
        {
            services.AddRegisterUser();
            services.AddLogin();
            services.AddGetUser();
            return services;
        }
    }
}
