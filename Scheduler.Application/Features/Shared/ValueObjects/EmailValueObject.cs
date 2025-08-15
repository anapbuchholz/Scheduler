using Scheduler.Application.Features.Shared.IO.Validation;

namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal sealed class EmailValueObject : BaseValueObject
    {
        private EmailValueObject(in string value, in int maxLength, in int minLength) : base(value, maxLength, minLength)
        {
        }

        public  bool IsValidEmail { get { return !Value.IsValidEmail(); } }

        public static EmailValueObject Create(in string email, in int maxLength = 255, in int minLength = 0)
        {
            return new EmailValueObject(email, maxLength, minLength);
        }
    }
}
