using AutoFixture;
using Moq;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.GetUser.UseCase;
using Scheduler.Application.Features.UseCases.User.Shared;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.GetUser.UseCase
{
    [TestClass]
    public class GetUserUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly GetUserUseCase _useCase;
        private readonly Fixture _fixture;

        public GetUserUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userSessionMock = new Mock<IUserSession>();
            _useCase = new GetUserUseCase(_userRepositoryMock.Object, _userSessionMock.Object);
            _fixture = new Fixture();
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserExistsAndIsAdmin_ReturnsOkResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userExternalId = Guid.NewGuid().ToString();
            var request = new GetUserRequest { UserId = userId };
            var user = _fixture.Build<UserEntity>()
                               .With(u => u.Id, userId)
                               .With(u => u.ExternalId, userExternalId)
                               .Create();
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                               .ReturnsAsync(user);
            _userSessionMock.Setup(x => x.IsAdmin)
                            .Returns(true);
            var expectedResponseBody = new GetUserResponse(user);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            AssertResponseBodyEqual(expectedResponseBody, response);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserDoesNotExist_ReturnsNotFoundResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetUserRequest { UserId = userId };
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                               .ReturnsAsync((UserEntity)null);
            // Act
            var response = await _useCase.ExecuteAsync(request);
            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsNotAdminAndAccessingItself_ReturnsOkResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userExternalId = Guid.NewGuid().ToString();
            var request = new GetUserRequest { UserId = userId };
            var user = _fixture.Build<UserEntity>()
                               .With(u => u.Id, userId)
                               .With(u => u.ExternalId, userExternalId)
                               .Create();

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                               .ReturnsAsync(user);

            _userSessionMock.Setup(x => x.IsAdmin)
                            .Returns(false);

            _userSessionMock.Setup(x => x.UserExternalId)
                            .Returns(userExternalId);
            var expectedResponseBody = new GetUserResponse(user);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            AssertResponseBodyEqual(expectedResponseBody, response);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsNotAdminAndAccessingAnotherUser_ReturnsForbiddenResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userExternalId = Guid.NewGuid().ToString();
            var anotherUserExternalId = Guid.NewGuid().ToString();
            var request = new GetUserRequest { UserId = userId };

            var user = _fixture.Build<UserEntity>()
                               .With(u => u.Id, userId)
                               .With(u => u.ExternalId, userExternalId)
                               .Create();

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                               .ReturnsAsync(user);

            _userSessionMock.Setup(x => x.IsAdmin)
                            .Returns(false);

            _userSessionMock.Setup(x => x.UserExternalId)
                            .Returns(anotherUserExternalId);
            // Act
            var response = await _useCase.ExecuteAsync(request);
            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenExceptionOccurs_ReturnsInternalErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new GetUserRequest { UserId = userId };
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                               .ThrowsAsync(new Exception("Database error"));
            // Act
            var response = await _useCase.ExecuteAsync(request);
            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(response.ValidationErrorMessage.Contains("Database error"));
        }

        private void AssertResponseBodyEqual(GetUserResponse expected, Response actual)
        {
            var responseBody = actual.Body as GetUserResponse;
            Assert.AreEqual(expected.Id, responseBody.Id);
            Assert.AreEqual(expected.Name, responseBody.Name);
            Assert.AreEqual(expected.DocumentNumber, responseBody.DocumentNumber);
            Assert.AreEqual(expected.Email, responseBody.Email);
            Assert.AreEqual(expected.ExternalId, responseBody.ExternalId);
            Assert.AreEqual(expected.IsAdmin, responseBody.IsAdmin);
            Assert.AreEqual(expected.CompanyId, responseBody.CompanyId);
            Assert.AreEqual(expected.CreatedAt, responseBody.CreatedAt);
        }
    }
}
