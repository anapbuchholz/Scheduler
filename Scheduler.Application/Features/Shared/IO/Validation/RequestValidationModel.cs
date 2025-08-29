using System.Collections.Generic;

namespace Scheduler.Application.Features.Shared.IO.Validation
{
    internal sealed class RequestValidationModel(List<string> errors)
    {
        public List<string> Errors { get; } = errors;
        public bool IsValid { get { return Errors.Count == 0; } }
        public string ErrorMessage { get { return string.Join(", ", Errors); } }
    }
}
