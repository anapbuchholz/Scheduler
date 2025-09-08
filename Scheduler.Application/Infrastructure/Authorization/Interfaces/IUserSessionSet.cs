namespace Scheduler.Application.Infrastructure.Authorization.Interfaces
{
    public interface IUserSessionSet
    {
        void SetUserSession(string userId, string userName, string userEmail, bool isAdmin);
    }
}
