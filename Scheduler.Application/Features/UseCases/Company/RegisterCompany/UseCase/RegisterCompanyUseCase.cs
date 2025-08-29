using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase
{
    internal sealed class RegisterCompanyUseCase(
        ICompanyRepository companyRepository,
        IRequestValidator<RegisterCompanyRequest> validator) 
        : IUseCase<RegisterCompanyRequest, Response>
    {
        private readonly ICompanyRepository _companyRepository = companyRepository;
        private readonly IRequestValidator<RegisterCompanyRequest> _validator = validator;

        public async Task<Response> ExecuteAsync(RegisterCompanyRequest input)
        {
            var validationResult = await _validator.Validate(input);
            if (!validationResult.IsValid)
            {
                return Response.CreateInvalidParametersResponse(validationResult.ErrorMessage);
            }

            var existingCompany = await _companyRepository.GetCompanyByDocumentNumberAsync(input.DocumentNumber!);
            if (existingCompany != null)
            {
                return Response.CreateConflictResponse("Já existe uma empresa cadastrada com esse número de documento.");
            }

            var entity = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                DocumentNumber = input.DocumentNumber!,
                TradeName = input.TradeName!,
                LegalName = input.LegalName!,
                Email = input.Email!,
                Phone = input.PhoneNumber!
            };
            await _companyRepository.RegisterCompanyAsync(entity);

            return Response.CreateCreatedResponse();
        }
    }
}
