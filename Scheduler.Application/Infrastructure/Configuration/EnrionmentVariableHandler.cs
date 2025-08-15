namespace Scheduler.Application.Infrastructure.Configuration
{
    public static class EnrionmentVariableHandler
    {
        public static string GetEnvironmentVariable(string variableName)
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Environment variable '{variableName}' is not set.");
            }
            return value;
        }
    }
}
