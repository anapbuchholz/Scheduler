using Scheduler.Application.Features.Shared.IO.Query;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.ListUsers.UseCase
{
    [ExcludeFromCodeCoverage]
    public sealed class ListUsersRequest(string? name, string? email, string? documentNumber, bool? isAdmin, int pageNumber, int pageSize) : QueryRequest(pageNumber, pageSize)
    {
        public string? Name { get; set; } = name;

        public string? Email { get; set; } = email;

        public string? DocumentNumber { get; set; } = documentNumber;

        public bool? IsAdmin { get; set; } = isAdmin;
    }
}
