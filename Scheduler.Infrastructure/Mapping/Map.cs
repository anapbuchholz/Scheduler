using Dapper.FluentMap.Mapping;
using Scheduler.Infrastructure.Models;

namespace Scheduler.Infrastructure.Mapping
{
    public class Map : EntityMap<Company>
    {
        public Map() 
        {
            Map(x => x.Id).ToColumn("id");
            Map(x => x.TradeName).ToColumn("trade_name");
            Map(x => x.LegalName).ToColumn("legal_name");
            Map(x => x.DocumentNumber).ToColumn("tax_id");
            Map(x => x.Email).ToColumn("email");
            Map(x => x.Phone).ToColumn("phone");
            Map(x => x.IsActive).ToColumn("is_active");
            Map(x => x.CreatedAt).ToColumn("created_at");
        }
    }
}
