using Scheduler.Application.Features.Shared.Cypher;
using System;

namespace Scheduler.UnitTests.Application.Features.Shared.Cypher
{
    [TestClass]
    public sealed class AESTests
    {
        private const string DefaultPassword = "fe7ffef2-f3cb-4ede-990f-13c5a80c4960";
        private const string DefaultSalt = "h30iqS71";
        private const string DefaultInitVector = "AGTmb13p@zay26uL";

        public AESTests()
        {
            Environment.SetEnvironmentVariable("CYPHER_AES_SALT", DefaultSalt);
            Environment.SetEnvironmentVariable("CYPHER_AES_INITVECTOR", DefaultInitVector);
        }

        [TestMethod]
        public void Encrypt_And_Decrypt_With_Defaults_ShouldReturnOriginalText()
        {
            var plainText = "Texto AES 123456";
            var encrypted = AES.Encrypt(plainText, DefaultPassword);
            var decrypted = AES.Decrypt(encrypted, DefaultPassword);
            Assert.AreEqual(plainText, decrypted);
        }

        [TestMethod]
        public void Encrypt_And_Decrypt_With_CustomSaltAndVector_ShouldReturnOriginalText()
        {
            var plainText = "AES custom 12345";
            var customSalt = "pFiIu88y";
            var customVector = "FEDCBA0987654321";
            var encrypted = AES.Encrypt(plainText, DefaultPassword, customSalt, customVector);
            var decrypted = AES.Decrypt(encrypted, DefaultPassword, customSalt, customVector);
            Assert.AreEqual(plainText, decrypted);
        }

        [TestMethod]
        public void Encrypt_ShouldReturnDifferentResultForDifferentPasswords()
        {
            var plainText = "Texto igual 1234";
            var encrypted1 = AES.Encrypt(plainText, "Senha1");
            var encrypted2 = AES.Encrypt(plainText, "Senha2");
            Assert.AreNotEqual(encrypted1, encrypted2);
        }

        [TestMethod]
        public void Encrypt_And_Decrypt_EmptyString_ShouldWork()
        {
            var plainText = string.Empty;
            var encrypted = AES.Encrypt(plainText, DefaultPassword);
            var decrypted = AES.Decrypt(encrypted, DefaultPassword);
            Assert.AreEqual(plainText, decrypted);
        }

        [TestMethod]
        public void Encrypt_And_Decrypt_SpecialCharacters_ShouldWork()
        {
            var plainText = "!@#$%¨&*()_+=[]";
            var encrypted = AES.Encrypt(plainText, DefaultPassword);
            var decrypted = AES.Decrypt(encrypted, DefaultPassword);
            Assert.AreEqual(plainText, decrypted);
        }

        [TestMethod]
        public void Decrypt_WithInvalidBase64_ShouldThrowFormatException()
        {
            var invalidCipherText = "TextoNãoBase64";
            Assert.ThrowsExactly<FormatException>(() =>
            {
                AES.Decrypt(invalidCipherText, DefaultPassword);
            });
        }

        [TestMethod]
        public void Encrypt_WithTextGreaterThan16Chars_ShouldThrowArgumentException()
        {
            var plainText = "Este texto tem mais de 16 caracteres";
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                AES.Encrypt(plainText, DefaultPassword);
            });
        }
    }
}