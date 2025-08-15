using System.Data;

namespace Scheduler.Application.Infrastructure.Data.Shared.Context
{
    internal interface IDataContext
    {
        IDbConnection GetConnection();
    }
}
