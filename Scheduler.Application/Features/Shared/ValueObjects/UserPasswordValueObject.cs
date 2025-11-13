namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal class UserPasswordValueObject : BaseValueObject
    {
        private const int _passwordMinLength = 6;
        private const int _passwordMaxLength = 16;

        private UserPasswordValueObject(in string value, in int maxLength, in int minLength) : base(value, maxLength, minLength)
        {
        }

        public static UserPasswordValueObject Create(string password)
        {
            return new UserPasswordValueObject(password, _passwordMaxLength, _passwordMinLength);
        }
    }
}
