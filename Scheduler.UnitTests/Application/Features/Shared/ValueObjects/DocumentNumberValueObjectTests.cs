using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scheduler.Application.Features.Shared.ValueObjects;

namespace Scheduler.UnitTests.Application.Features.Shared.ValueObjects
{
    [TestClass]
    public class DocumentNumberValueObjectTests
    {
        // CPF válido: 52998224725
        // CPF inválido: 12345678900
        // CNPJ válido: 04252011000110
        // CNPJ inválido: 12345678000199

        [TestMethod]
        public void Create_WithValidCpf_ShouldSetPropertiesCorrectly()
        {
            var cpf = "52998224725";
            var vo = DocumentNumberValueObject.Create(cpf);

            Assert.AreEqual(cpf, vo.Value);
            Assert.AreEqual(11, vo.Value.Length);
            Assert.IsTrue(vo.IsDigitOnly);
            Assert.IsTrue(vo.IsValid);
            Assert.IsTrue(vo.IsValidCpf);
            Assert.IsFalse(vo.IsValidCnpj);
            Assert.IsFalse(vo.IsNullOrEmpty);
            Assert.IsFalse(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsGreaterThanMaxLength);
            Assert.IsFalse(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithInvalidCpf_ShouldSetPropertiesCorrectly()
        {
            var cpf = "12345678900";
            var vo = DocumentNumberValueObject.Create(cpf);

            Assert.AreEqual(cpf, vo.Value);
            Assert.AreEqual(11, vo.Value.Length);
            Assert.IsTrue(vo.IsDigitOnly);
            Assert.IsFalse(vo.IsValid);
            Assert.IsFalse(vo.IsValidCpf);
            Assert.IsFalse(vo.IsValidCnpj);
        }

        [TestMethod]
        public void Create_WithValidCnpj_ShouldSetPropertiesCorrectly()
        {
            var cnpj = "04252011000110";
            var vo = DocumentNumberValueObject.Create(cnpj);

            Assert.AreEqual(cnpj, vo.Value);
            Assert.AreEqual(14, vo.Value.Length);
            Assert.IsTrue(vo.IsDigitOnly);
            Assert.IsTrue(vo.IsValid);
            Assert.IsFalse(vo.IsValidCpf);
            Assert.IsTrue(vo.IsValidCnpj);
            Assert.IsFalse(vo.IsNullOrEmpty);
            Assert.IsFalse(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsGreaterThanMaxLength);
            Assert.IsFalse(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithInvalidCnpj_ShouldSetPropertiesCorrectly()
        {
            var cnpj = "12345678000199";
            var vo = DocumentNumberValueObject.Create(cnpj);

            Assert.AreEqual(cnpj, vo.Value);
            Assert.AreEqual(14, vo.Value.Length);
            Assert.IsTrue(vo.IsDigitOnly);
            Assert.IsFalse(vo.IsValid);
            Assert.IsFalse(vo.IsValidCpf);
            Assert.IsFalse(vo.IsValidCnpj);
        }

        [TestMethod]
        public void Create_WithNonDigitCharacters_ShouldSetIsDigitOnlyFalse()
        {
            var doc = "52998A24725";
            var vo = DocumentNumberValueObject.Create(doc);

            Assert.AreEqual(doc, vo.Value);
            Assert.IsFalse(vo.IsDigitOnly);
            Assert.IsFalse(vo.IsValid);
            Assert.IsFalse(vo.IsValidCpf);
            Assert.IsFalse(vo.IsValidCnpj);
        }

        [TestMethod]
        public void Create_WithEmptyString_ShouldSetIsNullOrEmptyAndIsNullOrWhiteSpace()
        {
            var vo = DocumentNumberValueObject.Create("");

            Assert.AreEqual("", vo.Value);
            Assert.IsTrue(vo.IsNullOrEmpty);
            Assert.IsTrue(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsDigitOnly);
            Assert.IsFalse(vo.IsValid);
            Assert.IsFalse(vo.IsValidCpf);
            Assert.IsFalse(vo.IsValidCnpj);
            Assert.IsTrue(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithNull_ShouldSetIsNullOrEmptyAndIsNullOrWhiteSpace()
        {
            var vo = DocumentNumberValueObject.Create(null);

            Assert.IsNull(vo.Value);
            Assert.IsTrue(vo.IsNullOrEmpty);
            Assert.IsTrue(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsDigitOnly);
            Assert.IsFalse(vo.IsValid);
            Assert.IsFalse(vo.IsValidCpf);
            Assert.IsFalse(vo.IsValidCnpj);
            Assert.IsTrue(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithShorterThanMinLength_ShouldSetIsLessThanMinLength()
        {
            var doc = "12345";
            var vo = DocumentNumberValueObject.Create(doc);

            Assert.AreEqual(doc, vo.Value);
            Assert.IsTrue(vo.IsLessThanMinLength);
            Assert.IsFalse(vo.IsValid);
        }

        [TestMethod]
        public void Create_WithGreaterThanMaxLength_ShouldSetIsGreaterThanMaxLength()
        {
            var doc = "123456789012345";
            var vo = DocumentNumberValueObject.Create(doc);

            Assert.AreEqual(doc, vo.Value);
            Assert.IsTrue(vo.IsGreaterThanMaxLength);
            Assert.IsFalse(vo.IsValid);
        }
    }
}