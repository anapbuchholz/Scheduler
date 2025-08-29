using Dapper.FluentMap.Mapping;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity
{
    internal class CompanyEntityMap : EntityMap<CompanyEntity>
    {
        public CompanyEntityMap()
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
