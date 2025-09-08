using Scheduler.Application.Features.Shared.IO.Validation;

namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal sealed class DocumentNumberValueObject : BaseValueObject
    {
        private const int _maxLength = 14;
        private const int _minLength = 11;

        private DocumentNumberValueObject(in string value, in int maxLength, in int minLength) : base(value, maxLength, minLength)
        {
        }

        public bool IsDigitOnly { get { return Value != null && Value.IsDigitsOnly(); } }

        public bool IsValid { get { return Value != null && (Value.IsValidCpf() || Value.IsValidCnpj()); } }

        public bool IsValidCpf { get { return Value != null && Value.IsValidCpf(); } }

        public bool IsValidCnpj { get { return Value != null && Value.IsValidCnpj(); } }

        public static DocumentNumberValueObject Create(string documentNumber)
        {
            return new DocumentNumberValueObject(documentNumber, _maxLength, _minLength);
        }
    }
}