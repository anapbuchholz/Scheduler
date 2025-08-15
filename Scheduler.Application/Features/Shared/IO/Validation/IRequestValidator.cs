namespace Scheduler.Application.Features.Shared.IO.Validation
{
    internal interface IRequestValidator<TRequest> where TRequest : IInput
    {
        Task<RequestValidationModel> Validate(TRequest request);
    }
}
