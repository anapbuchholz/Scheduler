using System.Net;

namespace Scheduler.Application.Features.Shared.IO
{
    public class Output
    {
        private const string ForbiddenResultMessage = "Falta de permissão para realizar essa ação";

        private Output(in HttpStatusCode statusCode, in string? validationErrorMessage, in object? body)
        {
            StatusCode = statusCode;
            ValidationErrorMessage = validationErrorMessage;
            Body = body;
        }

        public HttpStatusCode StatusCode { get; set; }
        public string? ValidationErrorMessage { get; set; }
        public object? Body { get; set; }

        public static Output CreateForbiddenResult()
        {
            return new Output(HttpStatusCode.Forbidden, ForbiddenResultMessage, null);
        }

        public static Output CreateInvalidParametersResult(in string message)
        {
            return new Output(HttpStatusCode.BadRequest, message, null);
        }

        public static Output CreateConflictResult(in string message)
        {
            return new Output(HttpStatusCode.Conflict, message, null);
        }

        public static Output CreateCreatedResult(in object? body = null)
        {
            return new Output(HttpStatusCode.Created, null, body);
        }

        public static Output CreateOkResult(in object? body = null)
        {
            return new Output(HttpStatusCode.OK, null, body);
        }

        public static Output CreateInternalErrorResult(in string message)
        {
            return new Output(HttpStatusCode.InternalServerError, message, null);
        }

        public static Output CreateNotFoundResult()
        {
            return new Output(HttpStatusCode.NotFound, null, null);
        }
    }
}
