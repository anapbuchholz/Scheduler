using AutoFixture;
using Moq;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.UpdateUser.UseCase
{
    [TestClass]
    public class UpdateUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRequestValidator<UpdateUserRequest>> _validatorMock;
        private readonly Mock<IFireBaseAuthenticationService> _authServiceMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly Fixture _fixture;
        private readonly UpdateUserUseCase _useCase;

        public UpdateUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IRequestValidator<UpdateUserRequest>>();
            _authServiceMock = new Mock<IFireBaseAuthenticationService>();
            _userSessionMock = new Mock<IUserSession>();
            _fixture = new Fixture();

            Environment.SetEnvironmentVariable("CYPHER_AES_KEY", "UnitTestAesKey1234");
            Environment.SetEnvironmentVariable("CYPHER_AES_SALT", "UnitTestSalt1234");
            Environment.SetEnvironmentVariable("CYPHER_AES_INITVECTOR", "1234567890ABCDEF");

            _useCase = new UpdateUserUseCase(_userRepositoryMock.Object, _validatorMock.Object, _authServiceMock.Object, _userSessionMock.Object);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenRequestIsNotValid_ShouldReturnInvalidParametersResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            var mockedValidationResponse = new RequestValidationModel(["There is an error", "There is another error"]);
            _validatorMock.Setup(vm => vm.ValidateAsync(It.IsAny<UpdateUserRequest>())).ReturnsAsync(mockedValidationResponse);

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mockedValidationResponse.ErrorMessage, result.ValidationErrorMessage);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.IsNull(result.Body);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserNotFound_ShouldReturnNotFoundResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            var validValidation = new RequestValidationModel([]);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateUserRequest>()))
                          .ReturnsAsync(validValidation);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(testInput.Id))
                               .ReturnsAsync((UserEntity)null);

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenNonAdminTriesToUpdateAnotherUser_ShouldReturnForbiddenResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            var currentUser = _fixture.Create<UserEntity>();
            var validValidation = new RequestValidationModel(new List<string>());

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateUserRequest>())).ReturnsAsync(validValidation);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(testInput.Id)).ReturnsAsync(currentUser);
            _userSessionMock.Setup(s => s.IsAdmin).Returns(false);
            _userSessionMock.Setup(s => s.UserExternalId).Returns(Guid.NewGuid().ToString()); // diferente do currentUser

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenNonAdminTriesToChangeAdminStatus_ShouldReturnForbiddenResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            testInput.IsAdmin = true;

            var currentUser = _fixture.Create<UserEntity>();
            currentUser.IsAdmin = false;

            var validValidation = new RequestValidationModel([]);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateUserRequest>())).ReturnsAsync(validValidation);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(testInput.Id)).ReturnsAsync(currentUser);
            _userSessionMock.Setup(s => s.IsAdmin).Returns(false);
            _userSessionMock.Setup(s => s.UserExternalId).Returns(currentUser.ExternalId);

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenTryingToChangeDocumentNumberAndItAlreadyExists_ShouldReturnConflictResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            var currentUser = _fixture.Create<UserEntity>();
            testInput.DocumentNumber = "99999999999";
            currentUser.DocumentNumber = "88888888888";

            var validValidation = new RequestValidationModel([]);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateUserRequest>())).ReturnsAsync(validValidation);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(testInput.Id)).ReturnsAsync(currentUser);
            _userRepositoryMock.Setup(r => r.GetUserByDocumentNumberAsync(It.IsAny<string>()))
                               .ReturnsAsync(_fixture.Create<UserEntity>());
            _userSessionMock.Setup(s => s.IsAdmin).Returns(true);

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenFirebaseUpdateFails_ShouldReturnInternalErrorResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            testInput.Password = "newpassword";

            var currentUser = _fixture.Create<UserEntity>();
            var validValidation = new RequestValidationModel([]);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateUserRequest>())).ReturnsAsync(validValidation);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(testInput.Id)).ReturnsAsync(currentUser);
            _userSessionMock.Setup(s => s.IsAdmin).Returns(true);
            _authServiceMock.Setup(a => a.UpdateFireBaseUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                            .ReturnsAsync((null, false));

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenEverythingIsValid_ShouldReturnOkResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            testInput.Password = "newpassword";

            var currentUser = _fixture.Create<UserEntity>();
            var validValidation = new RequestValidationModel([]);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateUserRequest>())).ReturnsAsync(validValidation);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(testInput.Id)).ReturnsAsync(currentUser);
            _authServiceMock.Setup(a => a.UpdateFireBaseUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                            .ReturnsAsync((Guid.NewGuid().ToString(), true));
            _userSessionMock.Setup(s => s.IsAdmin).Returns(true);

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(currentUser.Id, currentUser), Times.Once);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenExceptionIsThrown_ShouldReturnInternalErrorResponse()
        {
            //Arrange
            var testInput = _fixture.Create<UpdateUserRequest>();
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateUserRequest>()))
                          .ThrowsAsync(new Exception("Database crashed"));

            //Act
            var result = await _useCase.ExecuteAsync(testInput);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Contains("Database crashed", result.ValidationErrorMessage);
        }
    }
}
