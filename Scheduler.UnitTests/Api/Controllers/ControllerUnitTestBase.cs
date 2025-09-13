using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Scheduler.UnitTests.Api.Controllers
{
    public abstract class ControllerUnitTestBase
    {
        protected static void AssertHttpResponse<T>(ObjectResult currentResponse, HttpStatusCode expectedStatusCode, T expectedBody)
        {
            var value = currentResponse.Value;
            var bodyProperty = value.GetType().GetProperty("Body");
            var responseBody = bodyProperty?.GetValue(value);
            var currentBody = (T)responseBody;
            var currentCode = (HttpStatusCode)currentResponse.StatusCode!;

            Assert.AreEqual(expectedStatusCode, currentCode);
            Assert.AreEqual(expectedBody, currentBody);
        }
    }
}
