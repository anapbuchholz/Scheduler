using Microsoft.AspNetCore.Mvc;
using Scheduler.Application.Features.Shared.IO;
using System;
using System.Net;

namespace Scheduler.UnitTests.Api.Controllers.BaseTests
{
    [TestClass]
    public sealed class BaseControllerTests
    {
        private readonly ControllerMock controllerMock;

        public BaseControllerTests()
        {
            controllerMock = new ControllerMock();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest, typeof(BadRequestObjectResult))]
        [DataRow(HttpStatusCode.Conflict, typeof(ConflictObjectResult))]
        [DataRow(HttpStatusCode.Unauthorized, typeof(UnauthorizedResult))]
        [DataRow(HttpStatusCode.Forbidden, typeof(ForbidResult))]
        [DataRow(HttpStatusCode.NotFound, typeof(NotFoundObjectResult))]
        [DataRow(HttpStatusCode.OK, typeof(OkObjectResult))]
        [DataRow(HttpStatusCode.Created, typeof(CreatedResult))]
        [DataRow(HttpStatusCode.InternalServerError, typeof(ObjectResult))]
        [DataRow(HttpStatusCode.NoContent, typeof(NoContentResult))]
        public void GetHttpResponse_ShouldReturnExpectedActionResult(HttpStatusCode statusCode, Type expectedResultType)
        {
            // Arrange
            Response response;
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest:
                    response = Response.CreateInvalidParametersResponse("Invalid parameters");
                    break;
                case HttpStatusCode.Conflict:
                    response = Response.CreateConflictResponse("Conflict occurred");
                    break;
                case HttpStatusCode.Unauthorized:
                    response = Response.CreateUnauthorizedResponse();
                    break;
                case HttpStatusCode.Forbidden:
                    response = Response.CreateForbiddenResponse();
                    break;
                case HttpStatusCode.NotFound:
                    response = Response.CreateNotFoundResponse();
                    break;
                case HttpStatusCode.OK:
                    response = Response.CreateOkResponse(new { Message = "Success" });
                    break;
                case HttpStatusCode.Created:
                    response = Response.CreateCreatedResponse(new { Id = 1 });
                    break;
                case HttpStatusCode.InternalServerError:
                    response = Response.CreateInternalErrorResponse("Internal error");
                    break;
                default:
                    response = Response.CreateNoContentResponse();
                    break;
            }
            var uri = "http://example.com/resource";

            // Act
            var result = controllerMock.InvokeGetHttpResponse(response, uri);

            // Assert
            Assert.IsInstanceOfType(result, expectedResultType);
        }
    }
}
