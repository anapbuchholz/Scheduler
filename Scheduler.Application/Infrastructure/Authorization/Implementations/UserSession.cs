using Scheduler.Application.Infrastructure.Authorization.Interfaces;

namespace Scheduler.Application.Infrastructure.Authorization.Implementations
{
    internal class UserSession : IUserSession, IUserSessionSet
    {
        public string UserName { get; private set; } = string.Empty;

        public string UserEmail { get; private set; } = string.Empty;

        public string UserExternalId { get; private set; } = string.Empty;

        public bool IsAdmin { get; private set; } = false;

        public void SetUserSession(string userId, string userName, string userEmail, bool isAdmin)
        {
            UserExternalId = userId;
            UserName = userName;
            UserEmail = userEmail;
            IsAdmin = isAdmin;
        }
    }
}
