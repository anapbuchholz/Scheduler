using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User
{
    [ExcludeFromCodeCoverage]
    internal static class UserSqlConstants
    {
        public static string INSERT_USER = @"INSERT INTO scheduler.users 
                                            (id, name, tax_id, email, password_hash, is_admin, company_id, created_at, external_id) 
                                            VALUES (@Id, @Name, @DocumentNumber, @Email, @PasswordHash, @IsAdmin, @CompanyId, @CreatedAt, @ExternalId)";

        public static string SELECT_USER_BY_ID = @"SELECT 
                                                        id AS Id, 
                                                        name AS Name,
                                                        tax_id AS DocumentNumber,
                                                        email AS Email,
                                                        password_hash AS PasswordHash,
                                                        is_admin AS IsAdmin,
                                                        company_id AS CompanyId,
                                                        created_at AS CreatedAt,
                                                        external_id AS ExternalId
                                                    FROM 
                                                        scheduler.users 
                                                    WHERE 
                                                        id = @Id";

        public static string UPDATE_USER_BY_ID = @"UPDATE 
                                                        scheduler.users
                                                   SET
                                                        name = @Name,
                                                        tax_id = @DocumentNumber,
                                                        password_hash = @PasswordHash,
                                                        is_admin = @IsAdmin
                                                    WHERE
                                                        id = @Id;";

        public static string SELECT_USER_BY_EMAIL = @"SELECT 
                                                        id AS Id, 
                                                        name AS Name,
                                                        tax_id AS DocumentNumber,
                                                        email AS Email,
                                                        password_hash AS PasswordHash,
                                                        is_admin AS IsAdmin,
                                                        company_id AS CompanyId,
                                                        created_at AS CreatedAt,
                                                        external_id AS ExternalId
                                                    FROM 
                                                        scheduler.users 
                                                    WHERE 
                                                        email = @Email";

        public static string SELECT_USER_BY_DOCUMENT_NUMBER = @"SELECT 
                                                        id AS Id, 
                                                        name AS Name,
                                                        tax_id AS DocumentNumber,
                                                        email AS Email,
                                                        password_hash AS PasswordHash,
                                                        is_admin AS IsAdmin,
                                                        company_id AS CompanyId,
                                                        created_at AS CreatedAt,
                                                        external_id AS ExternalId
                                                    FROM 
                                                        scheduler.users 
                                                    WHERE 
                                                        tax_id = @DocumentNumber";

        public static class ListUsersPaginationConstants
        {
            public static string LIST_USERS_SELECT_STATEMENT = @"SELECT 
                                                        id AS Id, 
                                                        name AS Name,
                                                        tax_id AS DocumentNumber,
                                                        email AS Email,
                                                        password_hash AS PasswordHash,
                                                        is_admin AS IsAdmin,
                                                        company_id AS Company,
                                                        created_at AS CreatedAt,
                                                        external_id AS ExternalId";

            public static string LIST_USERS_FROM_STATEMENT = "FROM scheduler.users";

            public static string LIST_USERS_WHERE_STATEMENT = "WHERE 1=1";
        }
    }
}
