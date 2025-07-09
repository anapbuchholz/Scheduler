using Dapper.FluentMap;
using Scheduler.Infrastructure.Mapping;

namespace Scheduler.API
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
                    config.AddMap(new Map());
                });

                _isInitialized = true;
            }
        }
    }
}
