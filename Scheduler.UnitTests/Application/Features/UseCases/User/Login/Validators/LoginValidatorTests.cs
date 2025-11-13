using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using Scheduler.Application.Features.UseCases.User.Login.Validators;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.Login.Validators
{
    [TestClass]
    public class LoginValidatorTests
    {
        private LoginValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new LoginValidator();
        }

        [TestMethod]
        public async Task ValidateAsync_WithValidEmailAndPassword_ShouldBeValid()
        {
            var request = new LoginRequest
            {
                Email = "user@email.com",
                Password = "123456"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public async Task ValidateAsync_WithEmptyEmail_ShouldReturnEmailError()
        {
            var request = new LoginRequest
            {
                Email = "",
                Password = "123456"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve ser informado");
        }

        [TestMethod]
        public async Task ValidateAsync_WithNullEmail_ShouldReturnEmailError()
        {
            var request = new LoginRequest
            {
                Email = null,
                Password = "123456"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve ser informado");
        }

        [TestMethod]
        public async Task ValidateAsync_WithEmptyPassword_ShouldReturnPasswordError()
        {
            var request = new LoginRequest
            {
                Email = "user@email.com",
                Password = ""
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "A senha deve ser informada");
        }

        [TestMethod]
        public async Task ValidateAsync_WithNullPassword_ShouldReturnPasswordError()
        {
            var request = new LoginRequest
            {
                Email = "user@email.com",
                Password = null
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "A senha deve ser informada");
        }

        [TestMethod]
        public async Task ValidateAsync_WithEmptyEmailAndPassword_ShouldReturnBothErrors()
        {
            var request = new LoginRequest
            {
                Email = "",
                Password = ""
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve ser informado");
            CollectionAssert.Contains(result.Errors, "A senha deve ser informada");
        }

        [TestMethod]
        public async Task ValidateAsync_WithNullEmailAndPassword_ShouldReturnBothErrors()
        {
            var request = new LoginRequest
            {
                Email = null,
                Password = null
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve ser informado");
            CollectionAssert.Contains(result.Errors, "A senha deve ser informada");
        }
    }
}