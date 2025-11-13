namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal class UserNameValueObject : BaseValueObject
    {
        private const int _maxLength = 255;

        private UserNameValueObject(in string value, in int maxLength) : base(value, maxLength)
        {
        }

        public static UserNameValueObject Create(string userName)
        {
            return new UserNameValueObject(userName, _maxLength);
        }
    }
}
