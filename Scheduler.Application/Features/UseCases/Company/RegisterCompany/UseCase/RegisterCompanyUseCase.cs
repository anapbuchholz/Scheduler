using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase
{
    internal sealed class RegisterCompanyUseCase(
        ICompanyRepository companyRepository,
        IRequestValidator<RegisterCompanyInput> validator) 
        : IUseCase<RegisterCompanyInput, Output>
    {
        private readonly ICompanyRepository _companyRepository = companyRepository;
        private readonly IRequestValidator<RegisterCompanyInput> _validator = validator;

        public Task<Output> Execute(RegisterCompanyInput input)
        {
            throw new NotImplementedException();
        }
    }
}
