using Scheduler.Application.Features.Shared.ValueObjects;

namespace Scheduler.UnitTests.Application.Features.Shared.ValueObjects
{
    [TestClass]
    public class EmailValueObjectTests
    {
        [TestMethod]
        public void Create_WithValidEmail_ShouldSetPropertiesCorrectly()
        {
            var email = "usuario@email.com";
            var vo = EmailValueObject.Create(email);

            Assert.AreEqual(email, vo.Value);
            Assert.IsFalse(vo.IsNullOrEmpty);
            Assert.IsFalse(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsGreaterThanMaxLength);
            Assert.IsFalse(vo.IsLessThanMinLength);
            Assert.IsTrue(vo.IsValidEmail);
        }

        [TestMethod]
        public void Create_WithInvalidEmail_ShouldSetIsValidEmailFalse()
        {
            var email = "usuarioemail.com";
            var vo = EmailValueObject.Create(email);

            Assert.AreEqual(email, vo.Value);
            Assert.IsFalse(vo.IsNullOrEmpty);
            Assert.IsFalse(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsGreaterThanMaxLength);
            Assert.IsFalse(vo.IsLessThanMinLength);
            Assert.IsFalse(vo.IsValidEmail);
        }

        [TestMethod]
        public void Create_WithEmptyString_ShouldSetIsNullOrEmptyAndIsNullOrWhiteSpace()
        {
            var vo = EmailValueObject.Create("");

            Assert.AreEqual("", vo.Value);
            Assert.IsTrue(vo.IsNullOrEmpty);
            Assert.IsTrue(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsValidEmail);
            Assert.IsTrue(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithNull_ShouldSetIsNullOrEmptyAndIsNullOrWhiteSpace()
        {
            var vo = EmailValueObject.Create(null);

            Assert.IsNull(vo.Value);
            Assert.IsTrue(vo.IsNullOrEmpty);
            Assert.IsTrue(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsValidEmail);
            Assert.IsTrue(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithShorterThanMinLength_ShouldSetIsLessThanMinLength()
        {
            var email = "a@b";
            var vo = EmailValueObject.Create(email);

            Assert.AreEqual(email, vo.Value);
            Assert.IsTrue(vo.IsLessThanMinLength);
            Assert.IsFalse(vo.IsValidEmail);
        }

        [TestMethod]
        public void Create_WithGreaterThanMaxLength_ShouldSetIsGreaterThanMaxLength()
        {
            var local = new string('a', 250);
            var email = $"{local}@x.com"; // > 255 chars
            var vo = EmailValueObject.Create(email);

            Assert.AreEqual(email, vo.Value);
            Assert.IsTrue(vo.IsGreaterThanMaxLength);
            Assert.IsFalse(vo.IsValidEmail);
        }

        [TestMethod]
        public void Create_WithEmailWithSpaces_ShouldSetIsValidEmailFalse()
        {
            var email = "usuario @email.com";
            var vo = EmailValueObject.Create(email);

            Assert.AreEqual(email, vo.Value);
            Assert.IsFalse(vo.IsValidEmail);
        }

        [TestMethod]
        public void Create_WithEmailWithSpecialCharacters_ShouldSetIsValidEmailTrue()
        {
            var email = "us.er+test@dominio.com";
            var vo = EmailValueObject.Create(email);

            Assert.AreEqual(email, vo.Value);
            Assert.IsTrue(vo.IsValidEmail);
        }
    }
}