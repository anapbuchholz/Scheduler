using Dapper.FluentMap;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql
{
    public class DapperMappingInitializer
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new();

        public static void Initialize()
        {
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;

                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new CompanyEntityMap());
                });

                _isInitialized = true;
            }
        }
    }
}
