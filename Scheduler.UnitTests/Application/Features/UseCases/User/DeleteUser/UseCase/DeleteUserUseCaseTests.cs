using AutoFixture;
using Moq;
using Scheduler.Application.Features.UseCases.User.DeleteUser.UseCase;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.DeleteUser.UseCase
{
    [TestClass]
    public class DeleteUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IFireBaseAuthenticationService> _authServiceMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly Fixture _fixture;
        private readonly DeleteUserUseCase _useCase;

        public DeleteUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authServiceMock = new Mock<IFireBaseAuthenticationService>();
            _userSessionMock = new Mock<IUserSession>();
            _fixture = new Fixture();

            _useCase = new DeleteUserUseCase(_userRepositoryMock.Object, _authServiceMock.Object, _userSessionMock.Object);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserSessionIsNotAdmin_ShouldReturnNotFoundResponse()
        {
            // Arrange
            var input = _fixture.Create<DeleteUserRequest>();
            _userSessionMock.Setup(u => u.IsAdmin).Returns(false);

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            _userRepositoryMock.Verify(r => r.GetUserByIdAsync(It.IsAny<Guid>()), Times.Never);
            _authServiceMock.Verify(a => a.DeleteFireBaseUserAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserNotFound_ShouldReturnNotFoundResponse()
        {
            // Arrange
            var input = _fixture.Create<DeleteUserRequest>();
            _userSessionMock.Setup(u => u.IsAdmin).Returns(true);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(input.Id)).ReturnsAsync((UserEntity)null);

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            _authServiceMock.Verify(a => a.DeleteFireBaseUserAsync(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.DeleteUserAsync(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenFirebaseDeletionFails_ShouldReturnInternalErrorResponse()
        {
            // Arrange
            var input = _fixture.Create<DeleteUserRequest>();
            var user = new UserEntity { Email = "test@example.com" };

            _userSessionMock.Setup(u => u.IsAdmin).Returns(true);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(input.Id)).ReturnsAsync(user);
            _authServiceMock.Setup(a => a.DeleteFireBaseUserAsync(user.Email)).ReturnsAsync((null, false));

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Não foi possível excluir o usuário", result.ValidationErrorMessage);
            _userRepositoryMock.Verify(r => r.DeleteUserAsync(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenEverythingSucceeds_ShouldReturnOkResponse()
        {
            // Arrange
            var input = _fixture.Create<DeleteUserRequest>();
            var user = new UserEntity { Email = "ok@example.com" };

            _userSessionMock.Setup(u => u.IsAdmin).Returns(true);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(input.Id)).ReturnsAsync(user);
            _authServiceMock.Setup(a => a.DeleteFireBaseUserAsync(user.Email)).ReturnsAsync(("firebase-id", true));

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            _userRepositoryMock.Verify(r => r.DeleteUserAsync(input.Id), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUnexpectedExceptionOccurs_ShouldReturnInternalErrorResponse()
        {
            // Arrange
            var input = _fixture.Create<DeleteUserRequest>();
            _userSessionMock.Setup(u => u.IsAdmin).Returns(true);
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            StringAssert.Contains(result.ValidationErrorMessage, "DeleteUserUseCase->ExecuteAsync: DB failure");
        }
    }
}
