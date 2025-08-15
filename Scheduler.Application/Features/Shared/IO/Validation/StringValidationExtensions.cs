using System.Text.RegularExpressions;

namespace Scheduler.Application.Features.Shared.IO.Validation
{
    internal static class StringValidationExtensions
    {
        private static readonly string VALID_EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|br)$";

        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static bool IsValidEmail(this string email)
        {
            return Regex.IsMatch(email, VALID_EMAIL_REGEX, RegexOptions.IgnoreCase);
        }

        public static bool IsGreaterThanLimit(this string str, int length)
        {
            return str.Length > length;
        }

        public static bool IsLessThanLimit(this string str, int length)
        {
            return str.Length < length;
        }
    }
}
