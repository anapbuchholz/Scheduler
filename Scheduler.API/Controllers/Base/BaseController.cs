using Microsoft.AspNetCore.Mvc;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Infrastructure.Configuration;
using System.Net;

namespace Scheduler.API.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult GetHttpResponse(Response response, in string? uri = null)
        {
            var httpResponse = new { response.StatusCode, response.ValidationErrorMessage, response.Body };

            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => BadRequest(httpResponse),
                HttpStatusCode.Conflict => Conflict(httpResponse),
                HttpStatusCode.Unauthorized => Unauthorized(),
                HttpStatusCode.Forbidden => Forbid(),
                HttpStatusCode.NotFound => NotFound(httpResponse),
                HttpStatusCode.OK => Ok(httpResponse),
                HttpStatusCode.Created => Created(uri!, httpResponse),
                HttpStatusCode.InternalServerError => StatusCode(500, EnrionmentVariableHandler.IsDevelopment() ? response.ValidationErrorMessage : "Ocorreu um erro inesperado"),
                _ => NoContent()
            };
        }
    }
}
