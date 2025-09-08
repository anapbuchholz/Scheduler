namespace Scheduler.Application.Infrastructure.Authorization.Interfaces
{
    internal interface IUserSession
    {
        public string UserName { get; }
        
        public string UserEmail { get; }
        
        public string UserId { get; }
        
        public bool IsAdmin { get; }
    }
}
