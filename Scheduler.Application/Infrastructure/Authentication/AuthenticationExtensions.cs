using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Authentication.Attributes;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase;
using Scheduler.Application.Infrastructure.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Authentication
{
    [ExcludeFromCodeCoverage]
    internal static class AuthenticationExtensions
    {
        public static IServiceCollection AddFireBaseAuthentication(this IServiceCollection services)
        {
            AddFirebaseAdmin(services);
            AddFireBaseRestApiService(services);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandlerAttribute>(JwtBearerDefaults.AuthenticationScheme, (o) => { });

            return services;
        }

        private static void AddFirebaseAdmin(IServiceCollection services)
        {
            var credentials = new JsonCredentialParameters
            {
                Type = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_TYPE"),
                ProjectId = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_PROJECT_ID"),
                PrivateKeyId = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_PRIVATE_KEY_ID"),
                PrivateKey = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_PRIVATE_KEY"),
                ClientEmail = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_CLIENT_EMAIL"),
                ClientId = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_CLIENT_ID"),
                TokenUri = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_TOKEN_URI"),
                UniverseDomain = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_ACCOUNT_UNIVERSE_DOMAIN")
            };

            services.AddSingleton(FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJsonParameters(credentials)
            }));
        }

        private static void AddFireBaseRestApiService(IServiceCollection services)
        {
            var apiKey = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_API_KEY");
            var firebaseBasePath = EnvironmentVariableHandler.GetEnvironmentVariable("FIREBASE_API_BASE_PATH");
            var fireBaseAuthUri = $"{firebaseBasePath}{apiKey}";
            services.AddHttpClient<IFireBaseAuthenticationService, FireBaseAuthenticationService>(client =>
            {
                client.BaseAddress = new Uri(fireBaseAuthUri);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "SchedulerApp");
            });
        }
    }
}
