using Scheduler.Application.Features.Shared.ValueObjects;

namespace Scheduler.UnitTests.Application.Features.Shared.ValueObjects
{
    [TestClass]
    public class PhoneNumberValueObjectTests
    {
        [TestMethod]
        public void Create_WithValidPhoneNumber_ShouldSetPropertiesCorrectly()
        {
            var phone = "11999999999";
            var vo = PhoneNumberValueObject.Create(phone);

            Assert.AreEqual(phone, vo.Value);
            Assert.IsTrue(vo.IsDigitOnly);
            Assert.IsFalse(vo.IsNullOrEmpty);
            Assert.IsFalse(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsGreaterThanMaxLength);
            Assert.IsFalse(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithPhoneNumberWithNonDigits_ShouldSetIsDigitOnlyFalse()
        {
            var phone = "11-99999-9999";
            var vo = PhoneNumberValueObject.Create(phone);

            Assert.AreEqual(phone, vo.Value);
            Assert.IsFalse(vo.IsDigitOnly);
        }

        [TestMethod]
        public void Create_WithEmptyString_ShouldSetIsNullOrEmptyAndIsNullOrWhiteSpace()
        {
            var vo = PhoneNumberValueObject.Create("");

            Assert.AreEqual("", vo.Value);
            Assert.IsTrue(vo.IsNullOrEmpty);
            Assert.IsTrue(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsDigitOnly);
            Assert.IsTrue(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithNull_ShouldSetIsNullOrEmptyAndIsNullOrWhiteSpace()
        {
            var vo = PhoneNumberValueObject.Create(null);

            Assert.IsNull(vo.Value);
            Assert.IsTrue(vo.IsNullOrEmpty);
            Assert.IsTrue(vo.IsNullOrWhiteSpace);
            Assert.IsFalse(vo.IsDigitOnly);
            Assert.IsTrue(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithShorterThanMinLength_ShouldSetIsLessThanMinLength()
        {
            var phone = "12345";
            var vo = PhoneNumberValueObject.Create(phone);

            Assert.AreEqual(phone, vo.Value);
            Assert.IsTrue(vo.IsLessThanMinLength);
        }

        [TestMethod]
        public void Create_WithGreaterThanMaxLength_ShouldSetIsGreaterThanMaxLength()
        {
            var phone = "119999999991234567890";
            var vo = PhoneNumberValueObject.Create(phone);

            Assert.AreEqual(phone, vo.Value);
            Assert.IsTrue(vo.IsGreaterThanMaxLength);
        }
    }
}