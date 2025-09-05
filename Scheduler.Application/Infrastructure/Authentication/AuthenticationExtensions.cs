using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Authentication.Services;
using Scheduler.Application.Infrastructure.Configuration;

namespace Scheduler.Application.Infrastructure.Authentication
{
    internal static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            services.AddSingleton<IFireBaseAuthenticationService, FireBaseAuthenticationService>();
            AddFirebaseAdmin();
            return services;
        }

        private static void AddFirebaseAdmin()
        {
            var credentials = new JsonCredentialParameters
            {
                Type = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_TYPE"),
                ProjectId = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_PROJECT_ID"),
                PrivateKeyId = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_PRIVATE_KEY_ID"),
                PrivateKey = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_PRIVATE_KEY"),
                ClientEmail = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_CLIENT_EMAIL"),
                ClientId = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_CLIENT_ID"),
                TokenUri = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_TOKEN_URI"),
                UniverseDomain = EnrionmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_UNIVERSE_DOMAIN")
            };

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJsonParameters(credentials)
            });
        }
    }
}
