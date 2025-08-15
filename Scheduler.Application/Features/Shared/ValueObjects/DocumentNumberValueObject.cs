using Scheduler.Application.Features.Shared.IO.Validation;

namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal sealed class DocumentNumberValueObject : BaseValueObject
    {
        private DocumentNumberValueObject(in string value, in int maxLength, in int minLength) : base(value, maxLength, minLength)
        {
        }

        public bool IsDigitOnly { get { return Value != null && Value.IsDigitsOnly(); } }

        public static DocumentNumberValueObject Create(string documentNumber, int maxLength = 14, int minLength = 11)
        {
            return new DocumentNumberValueObject(documentNumber, maxLength, minLength);
        }
    }
}
