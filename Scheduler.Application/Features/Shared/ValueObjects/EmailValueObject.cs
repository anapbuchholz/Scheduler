using Scheduler.Application.Features.Shared.IO.Validation;

namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal sealed class EmailValueObject : BaseValueObject
    {
        private const int _maxLength = 255;
        private const int _minLength = 5;

        private EmailValueObject(in string value, in int maxLength, in int minLength) : base(value, maxLength, minLength)
        {
        }

        public  bool IsValidEmail { get { return !IsLessThanMinLength && !IsGreaterThanMaxLength && Value.IsValidEmail(); } }

        public static EmailValueObject Create(in string email)
        {
            return new EmailValueObject(email, _maxLength, _minLength);
        }
    }
}
