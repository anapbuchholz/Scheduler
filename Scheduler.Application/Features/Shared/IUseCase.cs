using Scheduler.Application.Features.Shared.IO;

namespace Scheduler.Application.Features.Shared
{
    public interface IUseCase<TInput, TOutput> where TInput : IInput where TOutput : Output
    {
        Task<Output> Execute(TInput input);
    }
}
