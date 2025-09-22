using Scheduler.Application.Features.Shared.IO;
using System.Net;

namespace Scheduler.UnitTests.Application.Features.IO
{
    [TestClass]
    public class ResponseTests
    {
        [TestMethod]
        public void CreateForbiddenResponse_ShouldSetStatusCodeAndMessage()
        {
            var response = Response.CreateForbiddenResponse();
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.AreEqual("Falta de permissão para realizar essa ação", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateUnauthorizedResponse_ShouldSetStatusCode()
        {
            var response = Response.CreateUnauthorizedResponse();
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateInvalidParametersResponse_ShouldSetStatusCodeAndMessage()
        {
            var msg = "Parâmetros inválidos";
            var response = Response.CreateInvalidParametersResponse(msg);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(msg, response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateConflictResponse_ShouldSetStatusCodeAndMessage()
        {
            var msg = "Conflito detectado";
            var response = Response.CreateConflictResponse(msg);
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
            Assert.AreEqual(msg, response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateCreatedResponse_WithBody_ShouldSetStatusCodeAndBody()
        {
            var body = new { Id = 1, Name = "Test" };
            var response = Response.CreateCreatedResponse(body);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.AreEqual(body, response.Body);
        }

        [TestMethod]
        public void CreateCreatedResponse_WithoutBody_ShouldSetStatusCodeAndNullBody()
        {
            var response = Response.CreateCreatedResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateOkResponse_WithBody_ShouldSetStatusCodeAndBody()
        {
            var body = new { Success = true };
            var response = Response.CreateOkResponse(body);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.AreEqual(body, response.Body);
        }

        [TestMethod]
        public void CreateOkResponse_WithoutBody_ShouldSetStatusCodeAndNullBody()
        {
            var response = Response.CreateOkResponse();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateInternalErrorResponse_ShouldSetStatusCodeAndMessage()
        {
            var msg = "Erro interno";
            var response = Response.CreateInternalErrorResponse(msg);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(msg, response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateNotFoundResponse_ShouldSetStatusCode()
        {
            var response = Response.CreateNotFoundResponse();
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public void CreateNoContentResponse_ShouldSetStatusCode()
        {
            var response = Response.CreateNoContentResponse();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
        }
    }
}
