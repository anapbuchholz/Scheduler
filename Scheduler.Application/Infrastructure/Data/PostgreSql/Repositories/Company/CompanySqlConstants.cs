namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company
{
    internal static class CompanySqlConstants
    {
        public static string SELECT_COMPANY_BY_ID = @"SELECT
                                                    id as Id,
                                                    trade_name as TradeName,
                                                    legal_name as LegalName,
                                                    tax_id as DocumentNumber,
                                                    email as Email,
                                                    phone as Phone,
                                                    is_active as IsActive,
                                                    created_at as CreatedAt
                                                FROM 
                                                    scheduler.companies
                                                WHERE 
                                                    id = @Id";

        public static string INSERT_COMPANY = @"INSERT INTO scheduler.companies (id, trade_name, legal_name, tax_id, email, phone, is_active, created_at)
                                        VALUES (@Id, @TradeName, @LegalName, @DocumentNumber, @Email, @Phone, @IsActive, @CreatedAt)";
    }
}
