using Scheduler.Application.Features.Shared.IO.Validation;

namespace Scheduler.UnitTests.Application.Features.IO.Validation
{
    [TestClass]
    public class StringValidationExtensionsTests
    {
        [TestMethod]
        [DataRow("98431088095", true)] // Valid CPF
        [DataRow("123.456.789-09", true)] // Valid CPF with formatting
        [DataRow("111.111.111-11", false)] // All digits the same
        [DataRow("1234567890", false)] // Too short
        [DataRow("123456789012", false)] // Too long
        [DataRow("123.456.789-0a", false)] // Contains non-digit character
        [DataRow("", false)] // Empty string
        [DataRow(" ", false)] // White space
        public void IsValidCpf_ShouldReturnExpectedResult(string cpf, bool expected)
        {
            // Act
            bool result = cpf.IsValidCpf();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("12345678000195", true)] // Valid CNPJ
        [DataRow("12.345.678/0001-95", true)] // Valid CNPJ with formatting
        [DataRow("11.111.111/1111-11", false)] // All digits the same
        [DataRow("1234567800019", false)] // Too short
        [DataRow("123456780001950", false)] // Too long
        [DataRow("12.345.678/0001-9a", false)] // Contains non-digit character
        [DataRow("", false)] // Empty string
        [DataRow(" ", false)] // White space
        public void IsValidCnpj_ShouldReturnExpectedResult(string cnpj, bool expected)
        {
            // Act
            bool result = cnpj.IsValidCnpj();
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("abcd", 3, true)]
        [DataRow("    ", 3, true)]
        [DataRow("asd", 3, false)]
        [DataRow("as", 3, false)]
        [DataRow("", 3, false)]
        [DataRow(" ", 3, false)]
        public void IsGreaterThanLimit_ShouldReturnExpectedResult(string str, int maxLength, bool expected)
        {
            // Act
            bool result = str.IsGreaterThanLimit(maxLength);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("asd", 3, false)]
        [DataRow("abcd", 3, false)]
        [DataRow("    ", 3, false)]
        [DataRow("as", 3, true)]
        [DataRow("", 3, true)]
        [DataRow(" ", 3, true)]
        public void IsLessThanLimit_ShouldReturnExpectedResult(string str, int minLength, bool expected)
        {
            // Act
            bool result = str.IsLessThanLimit(minLength);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("123456", true)]
        [DataRow("123a456", false)]
        [DataRow("abcdefgh", false)]
        [DataRow("123456*&!", false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        public void IsDigitsOnly_ShouldReturnExpectedResult(string str, bool expected)
        {
            // Act
            var result  = str.IsDigitsOnly();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("email@domain.com", true)]
        [DataRow("3M41L@domain.com", true)]
        [DataRow("email@domain.com.br", true)]
        [DataRow("email@domain.com.*", false)]
        [DataRow("12345", false)]
        [DataRow("email@domain", false)]
        [DataRow("email@domain.c", false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        public void IsValidEmail_ShouldReturnExpectedResult(string str, bool expected)
        {
            // Act
            var result = str.IsValidEmail();

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
