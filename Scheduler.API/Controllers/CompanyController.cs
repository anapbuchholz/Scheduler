using Microsoft.AspNetCore.Mvc;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController() : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var company = await _GetCompanyUseCase.ExecuteAsync(id);
            if (company == null)
            {
                return NotFound(Response<object>.Fail("Company not found"));
            }

            return Ok(Response<object>.Ok(company));
        }

        [HttpPost]
        public IActionResult Create([FromBody] object company)
        {
            return CreatedAtAction(nameof(GetById), new { id = 0 }, company);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object company)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent();
        }
    }
}
