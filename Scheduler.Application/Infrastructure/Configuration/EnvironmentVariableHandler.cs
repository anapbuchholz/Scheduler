using System;

namespace Scheduler.Application.Infrastructure.Configuration
{
    public static class EnvironmentVariableHandler
    {
        private const string VARIABLE_NOT_SET_MESSAGE = "Missing Environment variable:";
        private const string ASPNETCORE_ENVIRONMENT_VARIABLE_NAME = "ASPNETCORE_ENVIRONMENT";

        public static string GetEnvironmentVariable(string variableName)
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"{VARIABLE_NOT_SET_MESSAGE} {variableName}");
            }

            return value;
        }

        public static bool IsDevelopment() => Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT_VARIABLE_NAME) == "Development";
    }
}
