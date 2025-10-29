using AutoFixture;
using Moq;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.RegisterUser.UseCase
{
    [TestClass]
    public class RegisterUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<IRequestValidator<RegisterUserRequest>> _validatorMock;
        private readonly Mock<IFireBaseAuthenticationService> _authServiceMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly Fixture _fixture;
        private readonly RegisterUserUseCase _useCase;

        public RegisterUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _validatorMock = new Mock<IRequestValidator<RegisterUserRequest>>();
            _authServiceMock = new Mock<IFireBaseAuthenticationService>();
            _userSessionMock = new Mock<IUserSession>();
            _fixture = new Fixture();

            // Set environment variable for AES key
            Environment.SetEnvironmentVariable("CYPHER_AES_KEY", "UnitTestAesKey1234");
            Environment.SetEnvironmentVariable("CYPHER_AES_SALT", "UnitTestSalt1234");
            Environment.SetEnvironmentVariable("CYPHER_AES_INITVECTOR", "1234567890ABCDEF");

            _useCase = new RegisterUserUseCase(
                _userRepositoryMock.Object,
                _companyRepositoryMock.Object,
                _validatorMock.Object,
                _authServiceMock.Object,
                _userSessionMock.Object
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsNotAdmin_ReturnsForbidden()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(false);
            var request = _fixture.Create<RegisterUserRequest>();

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.AreEqual("Falta de permissão para realizar essa ação", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<RegisterUserRequest>()), Times.Never);
            _userRepositoryMock.Verify(x => x.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenValidationFails_ReturnsInvalidParameters()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterUserRequest>();
            var errors = new List<string> { "Erro de validação" };
            var validationModel = new RequestValidationModel(errors);
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(validationModel.ErrorMessage, response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _userRepositoryMock.Verify(x => x.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserAlreadyExists_ReturnsConflict()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterUserRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            var existingUser = _fixture.Create<UserEntity>();
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(request.Email!)).ReturnsAsync(existingUser);

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
            Assert.AreEqual("Já existe um usuário cadastrado com esse email.", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _companyRepositoryMock.Verify(x => x.GetCompanyAsync(It.IsAny<Guid>()), Times.Never);
            _authServiceMock.Verify(x => x.RegisterFireBaseUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<UserEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenNonAdminAndCompanyDoesNotExist_ReturnsInvalidParameters()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Build<RegisterUserRequest>()
                .With(x => x.IsAdmin, false)
                .With(x => x.CompanyId, Guid.NewGuid())
                .Create();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(request.Email!)).ReturnsAsync((UserEntity)null);
            _companyRepositoryMock.Setup(x => x.GetCompanyAsync(request.CompanyId!.Value)).ReturnsAsync((CompanyEntity)null);

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("A empresa informada não existe.", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _authServiceMock.Verify(x => x.RegisterFireBaseUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<UserEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenRegisterFireBaseUserFails_ReturnsInternalError()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Build<RegisterUserRequest>()
                .With(x => x.IsAdmin, true)
                .With(x => x.CompanyId, (Guid?)null)
                .Create();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(request.Email!)).ReturnsAsync((UserEntity)null);
            _authServiceMock.Setup(x => x.RegisterFireBaseUserAsync(request.Email!, request.Password!, $"{request.Name}-1"))
                .ReturnsAsync((null, false));

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual("Não foi possível cadastrar o usuário.", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _userRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<UserEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenValidAdminRequest_ShouldRegisterAndReturnCreated()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Build<RegisterUserRequest>()
                .With(x => x.IsAdmin, true)
                .With(x => x.CompanyId, (Guid?)null)
                .With(x => x.Password, "StrongP@ssw0rd")
                .Create();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(request.Email!)).ReturnsAsync((UserEntity)null);
            _authServiceMock.Setup(x => x.RegisterFireBaseUserAsync(request.Email!, request.Password!, $"{request.Name}-1"))
                .ReturnsAsync(("externalId123", true));

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            _userRepositoryMock.Verify(x => x.RegisterUserAsync(It.Is<UserEntity>(
                u => u.Email == request.Email
                  && u.Name == request.Name
                  && u.IsAdmin == true
                  && u.CompanyId == null
                  && u.ExternalId == "externalId123"
                  && !string.IsNullOrEmpty(u.PasswordHash)
            )), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenValidNonAdminRequest_ShouldRegisterAndReturnCreated()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var companyId = Guid.NewGuid();
            var request = _fixture.Build<RegisterUserRequest>()
                .With(x => x.IsAdmin, false)
                .With(x => x.CompanyId, companyId)
                .With(x => x.Password, "StrongP@ssw0rd")
                .Create();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(request.Email!)).ReturnsAsync((UserEntity)null);
            _companyRepositoryMock.Setup(x => x.GetCompanyAsync(companyId)).ReturnsAsync(_fixture.Create<CompanyEntity>());
            _authServiceMock.Setup(x => x.RegisterFireBaseUserAsync(request.Email!, request.Password!, $"{request.Name}-0"))
                .ReturnsAsync(("externalId456", true));

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            _userRepositoryMock.Verify(x => x.RegisterUserAsync(It.Is<UserEntity>(
                u => u.Email == request.Email
                  && u.Name == request.Name
                  && u.IsAdmin == false
                  && u.CompanyId == companyId
                  && u.ExternalId == "externalId456"
                  && !string.IsNullOrEmpty(u.PasswordHash)
            )), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenExceptionThrown_ReturnsInternalError()
        {
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterUserRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(request.Email!)).ThrowsAsync(new Exception("DB error"));

            var response = await _useCase.ExecuteAsync(request);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("RegisterUserUseCase->ExecuteAsync", response.ValidationErrorMessage);
            Assert.Contains("DB error", response.ValidationErrorMessage);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenDocumentNumberAlreadyExists_ReturnsConflict()
        {
            //Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<RegisterUserRequest>();
            var validationModel = new RequestValidationModel([]);

            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);

            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(request.Email!)).ReturnsAsync((UserEntity)null);

            var existingUserByDocument = _fixture.Create<UserEntity>();
            _userRepositoryMock.Setup(x => x.GetUserByDocumentNumberAsync(request.DocumentNumber!)).ReturnsAsync(existingUserByDocument);

            //Act
            var response = await _useCase.ExecuteAsync(request);

            //Assert
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
            Assert.AreEqual("Já existe um usuário cadastrado com esse número de documento.", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _authServiceMock.Verify(x => x.RegisterFireBaseUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<UserEntity>()), Times.Never);
        }
    }
}