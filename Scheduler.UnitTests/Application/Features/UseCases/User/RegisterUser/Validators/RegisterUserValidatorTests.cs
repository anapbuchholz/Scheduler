using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using Scheduler.Application.Features.UseCases.User.RegisterUser.Validators;
using System;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.RegisterUser.Validators
{
    [TestClass]
    public class RegisterUserValidatorTests
    {
        private RegisterUserValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new RegisterUserValidator();
        }

        [TestMethod]
        public async Task ValidateAsync_AllFieldsValid_AdminUser_ShouldBeValid()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725", // CPF válido
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true,
                CompanyId = null
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public async Task ValidateAsync_AllFieldsValid_NonAdminUser_ShouldBeValid()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = false,
                CompanyId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public async Task ValidateAsync_NameEmpty_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "",
                DocumentNumber = "52998224725",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O nome deve ser informado.");
        }

        [TestMethod]
        public async Task ValidateAsync_NameTooLong_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = new string('A', 256),
                DocumentNumber = "52998224725",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O nome deve conter no máximo 255 caracteres.");
        }

        [TestMethod]
        public async Task ValidateAsync_PasswordEmpty_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "usuario@teste.com",
                Password = "",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "A senha deve ser informada.");
        }

        [TestMethod]
        public async Task ValidateAsync_PasswordTooShort_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "usuario@teste.com",
                Password = "123",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "A senha deve conter no mínimo 6 caracteres.");
        }

        [TestMethod]
        public async Task ValidateAsync_PasswordTooLong_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "usuario@teste.com",
                Password = new string('1', 17),
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "A senha deve conter no máximo 16 caracteres.");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberEmpty_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O número do documento deve ser informado.");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberWithNonDigits_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "12345A78901",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O número do documento deve conter apenas dígitos numéricos.");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberTooLong_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "123456789012345",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O número do documento deve conter no máximo 14 dígitos.");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberTooShort_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "12345",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O número do documento deve conter no mínimo 11 dígitos.");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberInvalidCpf_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "12345678900", // CPF inválido
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O número do documento informado é inválido.");
        }

        [TestMethod]
        public async Task ValidateAsync_EmailEmpty_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve ser informado.");
        }

        [TestMethod]
        public async Task ValidateAsync_EmailTooLong_ShouldReturnError()
        {
            var email = new string('a', 250) + "@mail.com";
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = email,
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve conter no máximo 255 caracteres.");
        }

        [TestMethod]
        public async Task ValidateAsync_EmailTooShort_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "a@b",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve conter no mínimo 5 caracteres.");
        }

        [TestMethod]
        public async Task ValidateAsync_EmailInvalidFormat_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "emailinvalido",
                Password = "123456",
                IsAdmin = true
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail informado é inválido.");
        }

        [TestMethod]
        public async Task ValidateAsync_NonAdminUserWithoutCompanyId_ShouldReturnError()
        {
            var request = new RegisterUserRequest
            {
                Name = "Usuário Teste",
                DocumentNumber = "52998224725",
                Email = "usuario@teste.com",
                Password = "123456",
                IsAdmin = false,
                CompanyId = null
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O Id da empresa deve ser informado para usuários não administradores.");
        }
    }
}