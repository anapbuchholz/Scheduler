using System.Net;

namespace Scheduler.Application.Features.Shared.IO
{
    public class Response
    {
        private const string FORBIDDEN_RESULT_MESSAGE = "Falta de permissão para realizar essa ação";

        private Response(in HttpStatusCode statusCode, in string? validationErrorMessage = null, in object? body = null)
        {
            StatusCode = statusCode;
            ValidationErrorMessage = validationErrorMessage;
            Body = body;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public string? ValidationErrorMessage { get; private set; }
        public object? Body { get; private set; }

        public static Response CreateForbiddenResponse()
        {
            return new Response(HttpStatusCode.Forbidden, FORBIDDEN_RESULT_MESSAGE, null);
        }

        public static Response CreateUnauthorizedResponse()
        {
            return new Response(HttpStatusCode.Unauthorized);
        }

        public static Response CreateInvalidParametersResponse(in string message)
        {
            return new Response(HttpStatusCode.BadRequest, message);
        }

        public static Response CreateConflictResponse(in string message)
        {
            return new Response(HttpStatusCode.Conflict, message);
        }

        public static Response CreateCreatedResponse(in object? body = null)
        {
            return new Response(HttpStatusCode.Created, body: body);
        }

        public static Response CreateOkResponse(in object? body = null)
        {
            return new Response(HttpStatusCode.OK, body: body);
        }

        public static Response CreateInternalErrorResponse(in string message)
        {
            return new Response(HttpStatusCode.InternalServerError, message);
        }

        public static Response CreateNotFoundResponse(in string? message = null)
        {
            return new Response(HttpStatusCode.NotFound, message);
        }

        public static Response CreateNoContentResponse()
        {
            return new Response(HttpStatusCode.NoContent);
        }
    }
}
