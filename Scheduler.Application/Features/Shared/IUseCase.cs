using Scheduler.Application.Features.Shared.IO;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.Shared
{
    public interface IUseCase<TInput, TOutput> where TInput : IRequest where TOutput : Response
    {
        Task<Response> Execute(TInput input);
    }
}
