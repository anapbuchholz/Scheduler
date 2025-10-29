using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Controllers.Base;
using Scheduler.Application.Features.Shared.IO;

namespace Scheduler.UnitTests.Api.Controllers.BaseTests
{
    internal sealed class ControllerMock : BaseController
    {
        public IActionResult InvokeGetHttpResponse(Response response, in string uri = null)
        {
            return GetHttpResponse(response, uri);
        }
    }
}
