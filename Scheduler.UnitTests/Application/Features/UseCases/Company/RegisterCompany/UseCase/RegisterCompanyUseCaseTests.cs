using AutoFixture;
using Moq;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.Company.RegisterCompany.UseCase
{
    [TestClass]
    public class RegisterCompanyUseCaseTests
    {
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<IRequestValidator<RegisterCompanyRequest>> _validatorMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly Fixture _fixture;
        private readonly RegisterCompanyUseCase _useCase;

        public RegisterCompanyUseCaseTests()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _validatorMock = new Mock<IRequestValidator<RegisterCompanyRequest>>();
            _userSessionMock = new Mock<IUserSession>();
            _fixture = new Fixture();
            _useCase = new RegisterCompanyUseCase(
                _companyRepositoryMock.Object,
                _validatorMock.Object,
                _userSessionMock.Object
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsNotAdmin_ReturnsForbidden()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(false);
            var request = _fixture.Create<RegisterCompanyRequest>();

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.AreEqual("Falta de permissão para realizar essa ação", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<RegisterCompanyRequest>()), Times.Never);
            _companyRepositoryMock.Verify(x => x.GetCompanyByDocumentNumberAsync(It.IsAny<string>()), Times.Never);
            _companyRepositoryMock.Verify(x => x.RegisterCompanyAsync(It.IsAny<CompanyEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenValidationFails_ReturnsInvalidParameters()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterCompanyRequest>();
            var errors = new List<string> { "Erro de validação" };
            var validationModel = new RequestValidationModel(errors);
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(validationModel.ErrorMessage, response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _companyRepositoryMock.Verify(x => x.GetCompanyByDocumentNumberAsync(It.IsAny<string>()), Times.Never);
            _companyRepositoryMock.Verify(x => x.RegisterCompanyAsync(It.IsAny<CompanyEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenCompanyAlreadyExists_ReturnsConflict()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterCompanyRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            var existingCompany = _fixture.Create<CompanyEntity>();
            _companyRepositoryMock.Setup(x => x.GetCompanyByDocumentNumberAsync(request.DocumentNumber!)).ReturnsAsync(existingCompany);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
            Assert.AreEqual("Já existe uma empresa cadastrada com esse número de documento.", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _companyRepositoryMock.Verify(x => x.RegisterCompanyAsync(It.IsAny<CompanyEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenValidRequest_ShouldRegisterAndReturnCreated()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterCompanyRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _companyRepositoryMock.Setup(x => x.GetCompanyByDocumentNumberAsync(request.DocumentNumber!)).ReturnsAsync((CompanyEntity)null);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            _companyRepositoryMock.Verify(x => x.RegisterCompanyAsync(It.Is<CompanyEntity>(
                c => c.DocumentNumber == request.DocumentNumber!.Trim()
                  && c.TradeName == request.TradeName!.Trim()
                  && c.LegalName == request.LegalName!.Trim()
                  && c.Email == request.Email.Trim()
                  && c.Phone == request.PhoneNumber.Trim()
                  && c.IsActive
                  && c.Id != Guid.Empty
            )), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenExceptionThrown_ReturnsInternalError()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterCompanyRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _companyRepositoryMock.Setup(x => x.GetCompanyByDocumentNumberAsync(request.DocumentNumber!)).ThrowsAsync(new Exception("DB error"));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(response.ValidationErrorMessage.Contains("RegisterCompanyUseCase->ExecuteAsync"));
            Assert.IsTrue(response.ValidationErrorMessage.Contains("DB error"));
        }
    }
}