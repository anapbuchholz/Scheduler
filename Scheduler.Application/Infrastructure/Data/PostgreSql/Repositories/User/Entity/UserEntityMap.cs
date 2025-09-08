using Dapper.FluentMap.Mapping;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity
{
    internal class UserEntityMap : EntityMap<UserEntity>
    {
        public UserEntityMap() 
        {
            Map(x => x.Id).ToColumn("id");
            Map(x => x.Name).ToColumn("name");
            Map(x => x.DocumentNumber).ToColumn("tax_id");
            Map(x => x.Email).ToColumn("email");
            Map(x => x.PasswordHash).ToColumn("password_hash");
            Map(x => x.IsAdmin).ToColumn("is_admin");
            Map(x => x.CompanyId).ToColumn("company_id");
            Map(x => x.CreatedAt).ToColumn("created_at");
            Map(x => x.ExternalId).ToColumn("external_id");
        }
    }
}
