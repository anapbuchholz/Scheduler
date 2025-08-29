using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Controllers.Base;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using System;
using System.Threading.Tasks;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CompaniesController(
        IUseCase<RegisterCompanyRequest, Response> registerCompanyUseCase,
        IUseCase<GetCompanyRequest, Response> getCompanyUsecase) : BaseController
    {
        private readonly IUseCase<RegisterCompanyRequest, Response> _registerCompanyUseCase = registerCompanyUseCase;
        private readonly IUseCase<GetCompanyRequest, Response> _getCompanyUseCase = getCompanyUsecase;

        [HttpGet]
        public Task<IActionResult> ListCompaniesAsync()
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyAsync([FromRoute] Guid id)
        {
            var response = await _getCompanyUseCase.ExecuteAsync(new GetCompanyRequest(id));
            return GetHttpResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCompanyAsync([FromBody] RegisterCompanyRequest input)
        {
            var response = await _registerCompanyUseCase.ExecuteAsync(input);
            return GetHttpResponse(response);
        }

        [HttpPatch("{id}")]
        public Task<IActionResult> UpdateCompanyAsync([FromRoute] Guid id, [FromBody] object company)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteCompanyAsync([FromRoute] Guid id)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }
    }
}
