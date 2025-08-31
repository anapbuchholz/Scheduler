namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User
{
    internal static class UserSqlConstants
    {
        public static string INSERT_USER = @"INSERT INTO scheduler.users 
                                            (id, name, tax_id, email, password_hash, is_admin, company_id, created_at) 
                                            VALUES (@Id, @Name, @DocumentNumber, @Email, @PasswordHash, @IsAdmin, @CompanyId, @CreatedAt)";

        public static string SELECT_USER_BY_EMAIL = @"SELECT 
                                                        id AS Id, 
                                                        name AS Name,
                                                        tax_id AS DocumentNumber,
                                                        email AS Email,
                                                        password_hash AS PasswordHash,
                                                        is_admin AS IsAdmin,
                                                        company_id AS CompanyId,
                                                        created_at AS CreatedAt 
                                                    FROM 
                                                        scheduler.users 
                                                    WHERE 
                                                        email = @Email";
    }
}
