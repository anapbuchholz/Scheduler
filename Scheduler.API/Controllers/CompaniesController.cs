using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Controllers.Base;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using System;
using System.Threading.Tasks;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CompaniesController(
        IUseCase<RegisterCompanyRequest, Response> registerCompanyUseCase) : BaseController
    {
        private readonly IUseCase<RegisterCompanyRequest, Response> _RegisterCompanyUseCase = registerCompanyUseCase;

        [HttpGet]
        public Task<IActionResult> ListCompaniesAsync()
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpGet("{id}")]
        public Task<IActionResult> GetCompanyAsync([FromRoute] Guid id)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCompanyAsync([FromBody] RegisterCompanyRequest input)
        {
            var response = await _RegisterCompanyUseCase.Execute(input);
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
