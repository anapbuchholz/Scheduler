using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.Validators;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.Company.RegisterCompany.Validators
{
    [TestClass]
    public class RegisterCompanyValidatorTests
    {
        private RegisterCompanyValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new RegisterCompanyValidator();
        }

        [TestMethod]
        public async Task ValidateAsync_AllFieldsValid_ShouldBeValid()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725", // CPF válido
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public async Task ValidateAsync_EmptyTradeName_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O nome fantasia deve ser informado");
        }

        [TestMethod]
        public async Task ValidateAsync_TradeNameTooLong_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = new string('A', 256),
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O nome fantasia deve conter no máximo 255 caracteres");
        }

        [TestMethod]
        public async Task ValidateAsync_EmptyLegalName_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "",
                DocumentNumber = "52998224725",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "A razão social deve ser informada");
        }

        [TestMethod]
        public async Task ValidateAsync_LegalNameTooLong_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = new string('B', 256),
                DocumentNumber = "52998224725",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "A razão social deve conter no máximo 255 caracteres");
        }

        [TestMethod]
        public async Task ValidateAsync_InvalidEmail_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = "emailinvalido",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail informado é inválido");
        }

        [TestMethod]
        public async Task ValidateAsync_EmailTooLong_ShouldReturnError()
        {
            var email = new string('a', 250) + "@mail.com";
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = email,
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve conter no máximo 255 caracteres");
        }

        [TestMethod]
        public async Task ValidateAsync_EmailTooShort_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = "a@b",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O E-mail deve conter no mínimo 5 caracteres");
        }

        [TestMethod]
        public async Task ValidateAsync_PhoneNumberTooLong_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = "empresa@teste.com",
                PhoneNumber = "119999999991234567890" // 21 dígitos
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O telefone deve conter no máximo 20 dígitos");
        }

        [TestMethod]
        public async Task ValidateAsync_PhoneNumberTooShort_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = "empresa@teste.com",
                PhoneNumber = "12345"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O telefone deve conter no mínimo 11 dígitos");
        }

        [TestMethod]
        public async Task ValidateAsync_PhoneNumberWithNonDigits_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "52998224725",
                Email = "empresa@teste.com",
                PhoneNumber = "11-99999-9999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O telefone deve conter apenas números");
        }

        [TestMethod]
        public async Task ValidateAsync_EmptyDocumentNumber_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O CNPJ/CPF deve ser informado");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberWithNonDigits_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "12345A78901",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O CNPJ/CPF deve conter apenas números");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberTooLong_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "123456789012345",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O CNPJ/CPF deve conter no máximo 14 dígitos");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberTooShort_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "12345",
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O CNPJ/CPF deve conter no mínimo 11 dígitos");
        }

        [TestMethod]
        public async Task ValidateAsync_DocumentNumberInvalidCpfOrCnpj_ShouldReturnError()
        {
            var request = new RegisterCompanyRequest
            {
                TradeName = "Empresa Teste",
                LegalName = "Empresa Teste LTDA",
                DocumentNumber = "12345678900", // CPF inválido
                Email = "empresa@teste.com",
                PhoneNumber = "11999999999"
            };

            var result = await _validator.ValidateAsync(request);

            Assert.IsFalse(result.IsValid);
            CollectionAssert.Contains(result.Errors, "O CNPJ/CPF informado é inválido");
        }
    }
}