namespace Scheduler.Application.Features.Shared.IO.Validation
{
    internal sealed class RequestValidationModel
    {
        public List<string> Errors { get; }
        public bool IsValid { get { return Errors.Count == 0; } }
        public string ErrorMessage { get { return string.Join(", ", Errors); } }

        public RequestValidationModel(List<string> errors)
        {
            Errors = errors;
        }
    }
}
