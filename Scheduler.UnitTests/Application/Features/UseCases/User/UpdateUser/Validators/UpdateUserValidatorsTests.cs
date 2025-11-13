using Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase;
using Scheduler.Application.Features.UseCases.User.UpdateUser.Validators;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.UpdateUser.Validators
{
    [TestClass]
    public class UpdateUserValidatorsTests
    {
        private UpdateUserValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new UpdateUserValidator();
        }

        [TestMethod]
        public async Task ValidateAsync_WhenAllFieldsAreNull_ShouldReturnValid()
        {
            var request = new UpdateUserRequest();
            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
            Assert.IsEmpty(result.Errors);
        }

        #region Name Tests

        [TestMethod]
        public async Task ValidateAsync_WhenNameIsEmpty_ShouldReturnInvalidNameError()
        {
            var request = new UpdateUserRequest { Name = "" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("O nome informado é inválido", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenNameExceedsMaxLength_ShouldReturnMaxLengthError()
        {
            var longName = new string('A', 300);
            var request = new UpdateUserRequest { Name = longName };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("O nome deve conter no máximo", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenNameIsValid_ShouldReturnValid()
        {
            var request = new UpdateUserRequest { Name = "Maria Silva" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
        }

        #endregion

        #region Document Number Tests

        [TestMethod]
        public async Task ValidateAsync_WhenDocumentHasLetters_ShouldReturnDigitOnlyError()
        {
            var request = new UpdateUserRequest { DocumentNumber = "12A45B" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("O número do documento deve conter apenas dígitos numéricos", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenDocumentTooLong_ShouldReturnMaxLengthError()
        {
            var request = new UpdateUserRequest { DocumentNumber = new string('1', 20) };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("O número do documento deve conter no máximo", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenDocumentTooShort_ShouldReturnMinLengthError()
        {
            var request = new UpdateUserRequest { DocumentNumber = "123" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("O número do documento deve conter no mínimo", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenDocumentHasValidLengthButIsInvalid_ShouldReturnInvalidDocumentError()
        {
            // Valor numérico correto em tamanho, mas inválido no formato (depende do ValueObject)
            var request = new UpdateUserRequest { DocumentNumber = "12345678900" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("O número do documento informado é inválido", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenDocumentNumberIsValid_ShouldReturnValid()
        {
            var request = new UpdateUserRequest { DocumentNumber = "12345678909" }; // Exemplo de CPF válido

            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
        }

        #endregion

        #region Password Tests

        [TestMethod]
        public async Task ValidateAsync_WhenPasswordIsEmpty_ShouldReturnInvalidPasswordError()
        {
            var request = new UpdateUserRequest { Password = "" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("A senha informada é inválida", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenPasswordTooShort_ShouldReturnMinLengthError()
        {
            var request = new UpdateUserRequest { Password = "123" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("A senha deve conter no mínimo", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenPasswordTooLong_ShouldReturnMaxLengthError()
        {
            var request = new UpdateUserRequest { Password = new string('A', 200) };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.Contains("A senha deve conter no máximo", result.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidateAsync_WhenPasswordIsValid_ShouldReturnValid()
        {
            var request = new UpdateUserRequest { Password = "SenhaSegura123" };

            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
        }

        #endregion

        [TestMethod]
        public async Task ValidateAsync_WhenMultipleFieldsInvalid_ShouldReturnAllErrors()
        {
            var request = new UpdateUserRequest
            {
                Name = "",
                DocumentNumber = "abc",
                Password = "1"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            Assert.IsGreaterThanOrEqualTo(3, result.Errors.Count);
            Assert.Contains("O nome informado é inválido", result.ErrorMessage);
            Assert.Contains("O número do documento deve conter apenas dígitos numéricos", result.ErrorMessage);
            Assert.Contains("A senha deve conter no mínimo", result.ErrorMessage);
        }
    }
}
