using System.Linq;
using System.Text.RegularExpressions;

namespace Scheduler.Application.Features.Shared.IO.Validation
{
    internal static class StringValidationExtensions
    {
        private static readonly string VALID_EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|br)$";

        public static bool IsDigitsOnly(this string str)
        {
            if(string.IsNullOrWhiteSpace(str))
                return false;

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

        public static bool IsValidCpf(this string cpf)
        {
            // Remove non-digit characters
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            // Check length
            if (cpf.Length != 11) return false;

            // Check if all digits are the same
            if (cpf.Distinct().Count() == 1) return false;

            // Calculate first check digit
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (cpf[i] - '0') * (10 - i);

            int remainder = sum % 11;
            int firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            // Calculate second check digit
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += (cpf[i] - '0') * (11 - i);

            remainder = sum % 11;
            int secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            // Compare with given digits
            return (cpf[9] - '0' == firstCheckDigit) && (cpf[10] - '0' == secondCheckDigit);
        }

        public static bool IsValidCnpj(this string cnpj)
        {
            // Remove non-digit characters
            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            // Check length
            if (cnpj.Length != 14) return false;

            // Check if all digits are the same
            if (cnpj.Distinct().Count() == 1) return false;

            // Weight arrays for check digits
            int[] firstWeights = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] secondWeights = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Calculate first check digit
            int sum = 0;
            for (int i = 0; i < 12; i++)
                sum += (cnpj[i] - '0') * firstWeights[i];

            int remainder = sum % 11;
            int firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            // Calculate second check digit
            sum = 0;
            for (int i = 0; i < 13; i++)
                sum += (cnpj[i] - '0') * secondWeights[i];

            remainder = sum % 11;
            int secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            // Compare with given digits
            return (cnpj[12] - '0' == firstCheckDigit) && (cnpj[13] - '0' == secondCheckDigit);
        }

        public static bool IsGreaterThanLimit(this string str, int maxLength)
        {
            return str.Length > maxLength;
        }

        public static bool IsLessThanLimit(this string str, int minLength)
        {
            return str.Length < minLength;
        }
    }
}
