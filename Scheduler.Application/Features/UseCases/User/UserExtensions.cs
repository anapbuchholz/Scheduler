using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.UseCases.User.DeleteUser;
using Scheduler.Application.Features.UseCases.User.GetUser;
using Scheduler.Application.Features.UseCases.User.ListUsers;
using Scheduler.Application.Features.UseCases.User.Login;
using Scheduler.Application.Features.UseCases.User.RegisterUser;
using Scheduler.Application.Features.UseCases.User.UpdateUser;
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
            services.AddListUsers();
            services.AddUpdateUser();
            services.AddDeleteUser();
            return services;
        }
    }
}
