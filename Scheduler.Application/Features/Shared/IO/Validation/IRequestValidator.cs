using System.Threading.Tasks;

namespace Scheduler.Application.Features.Shared.IO.Validation
{
    internal interface IRequestValidator<TRequest> where TRequest : IRequest
    {
        Task<RequestValidationModel> Validate(TRequest request);
    }
}
