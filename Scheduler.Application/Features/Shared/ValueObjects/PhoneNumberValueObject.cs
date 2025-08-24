using Scheduler.Application.Features.Shared.IO.Validation;

namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal sealed class PhoneNumberValueObject : BaseValueObject
    {
        private const int _maxLength = 20;
        private const int _minLength = 11;

        private PhoneNumberValueObject(in string value, in int maxLength, in int minLength) : base(value, maxLength, minLength)
        {
        }

        public bool IsDigitOnly { get { return Value != null && Value.IsDigitsOnly(); } }

        public static PhoneNumberValueObject Create(string documentNumber)
        {
            return new PhoneNumberValueObject(documentNumber, _maxLength, _minLength);
        }
    }
}
