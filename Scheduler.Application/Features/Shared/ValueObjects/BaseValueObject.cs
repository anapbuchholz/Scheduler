using Scheduler.Application.Features.Shared.IO.Validation;

namespace Scheduler.Application.Features.Shared.ValueObjects
{
    internal abstract class BaseValueObject(in string value, in int maxLength, in int minLength)
    {
        public string Value { get; private set; } = value;
        public int MaxLength { get; private set; } = maxLength;
        public int MinLength { get; private set; } = minLength;


        public bool IsNullOrEmpty { get { return string.IsNullOrEmpty(Value); } }

        public bool IsNullOrWhiteSpace { get { return string.IsNullOrWhiteSpace(Value); } }

        public bool IsGreaterThanMaxLength { get { return Value != null && Value.IsGreaterThanLimit(MaxLength); } }

        public bool IsLessThanMinLength { get { return Value != null && Value.IsLessThanLimit(MaxLength); } }
    }
}
