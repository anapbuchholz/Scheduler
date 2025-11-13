using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Scheduler.API.Controllers;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.DeleteUser.UseCase;
using Scheduler.Application.Features.UseCases.User.GetUser.UseCase;
using Scheduler.Application.Features.UseCases.User.ListUsers.UseCase;
using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using Scheduler.Application.Features.UseCases.User.Shared;
using Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using Scheduler.UnitTests.Api.Controllers.Base;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Api.Controllers
{
    [TestClass]
    public sealed class UsersControllerTests : ControllerUnitTestBase
    {
        private readonly Mock<IUseCase<RegisterUserRequest, Response>> _registerUserUseCaseMock;
        private readonly Mock<IUseCase<LoginRequest, Response>> _loginUseCaseMock;
        private readonly Mock<IUseCase<GetUserRequest, Response>> _getUserUseCaseMock;
        private readonly Mock<IUseCase<ListUsersRequest, Response>> _listUsersUseCaseMock;
        private readonly Mock<IUseCase<UpdateUserRequest, Response>> _updateUsersUseCaseMock;
        private readonly Mock<IUseCase<DeleteUserRequest, Response>> _deleteUserUseCaseMock;
        private readonly Fixture _fixture;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _registerUserUseCaseMock = new Mock<IUseCase<RegisterUserRequest, Response>>();
            _loginUseCaseMock = new Mock<IUseCase<LoginRequest, Response>>();
            _getUserUseCaseMock = new Mock<IUseCase<GetUserRequest, Response>>();
            _listUsersUseCaseMock = new Mock<IUseCase<ListUsersRequest, Response>>();
            _updateUsersUseCaseMock = new Mock<IUseCase<UpdateUserRequest, Response>>();
            _deleteUserUseCaseMock = new Mock<IUseCase<DeleteUserRequest, Response>>();
            _fixture = new Fixture();

            _controller = new UsersController(
                _registerUserUseCaseMock.Object,
                _loginUseCaseMock.Object,
                _getUserUseCaseMock.Object,
                _listUsersUseCaseMock.Object,
                _updateUsersUseCaseMock.Object,
                _deleteUserUseCaseMock.Object
            );
        }

        #region LoginAsync

        [TestMethod]
        public async Task LoginAsync_WhenCalled_ShouldReturnOkWithToken()
        {
            // Arrange
            var requestMock = _fixture.Create<LoginRequest>();
            var responseBodyMock = new { Token = Guid.NewGuid().ToString() };
            var responseMock = Response.CreateOkResponse(responseBodyMock);

            _loginUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(responseMock);

            // Act
            var result = await _controller.LoginAsync(requestMock);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);

            AssertHttpResponse(resultValue, HttpStatusCode.OK, responseBodyMock);

            _loginUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<LoginRequest>(
                r => r.Email == requestMock.Email
                  && r.Password == requestMock.Password)), Times.Once);
        }

        #endregion

        #region RegisterUserAsync

        [TestMethod]
        public async Task RegisterUserAsync_WhenCalled_ShouldReturnCreatedUser()
        {
            // Arrange
            var requestMock = _fixture.Create<RegisterUserRequest>();
            var responseMock = Response.CreateCreatedResponse();

            _registerUserUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<RegisterUserRequest>()))
                .ReturnsAsync(responseMock);

            // Act
            var result = await _controller.RegisterUserAsync(requestMock);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            var resultValue = result as CreatedResult;
            Assert.IsNotNull(resultValue);

            AssertHttpResponse(resultValue, HttpStatusCode.Created);

            _registerUserUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<RegisterUserRequest>(
                r => r.Name == requestMock.Name
                  && r.DocumentNumber == requestMock.DocumentNumber
                  && r.Email == requestMock.Email
                  && r.Password == requestMock.Password
                  && r.IsAdmin == requestMock.IsAdmin
                  && r.CompanyId == requestMock.CompanyId)), Times.Once);
        }

        #endregion

        #region GetUserAsync

        [TestMethod]
        public async Task GetUserAsync_WhenCalled_ShouldReturnOkWithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseBodyMock = _fixture.Create<GetUserResponse>();
            var responseMock = Response.CreateOkResponse(responseBodyMock);
            _getUserUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<GetUserRequest>()))
                .ReturnsAsync(responseMock);

            // Act
            var result = await _controller.GetUserAsync(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);
            AssertHttpResponse(resultValue, HttpStatusCode.OK, responseBodyMock);
            _getUserUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<GetUserRequest>(
                r => r.UserId == userId)), Times.Once);
        }

        #endregion

        #region ListUsersAsync

        [TestMethod]
        public async Task ListUsersAsync_WhenCalled_ShouldReturnOkWithUsersList()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var responseBodyMock = _fixture.Create<PaginatedQueryResult<GetUserResponse>>();
            var responseMock = Response.CreateOkResponse(responseBodyMock);
            _listUsersUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<ListUsersRequest>()))
                .ReturnsAsync(responseMock);
            // Act
            var result = await _controller.ListUsersAsync(pageNumber, pageSize);
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);
            AssertHttpResponse(resultValue, HttpStatusCode.OK, responseBodyMock);
            _listUsersUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<ListUsersRequest>(
                r => r.PaginationParameters.PageNumber == pageNumber
                  && r.PaginationParameters.PageSize == pageSize)), Times.Once);
        }

        #endregion

        #region UpdateUserAsync

        [TestMethod]
        public async Task UpdateUserAsync_WhenCalled_ShouldReturnOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestMock = _fixture.Create<UpdateUserRequest>();
            var responseMock = Response.CreateOkResponse();
            _updateUsersUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<UpdateUserRequest>()))
                .ReturnsAsync(responseMock);

            // Act
            var result = await _controller.UpdateUserAsync(userId, requestMock);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);
            AssertHttpResponse(resultValue, HttpStatusCode.OK);
            _updateUsersUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<UpdateUserRequest>(
                r => r.Id == userId
                  && r.Name == requestMock.Name
                  && r.DocumentNumber == requestMock.DocumentNumber
                  && r.Password == requestMock.Password
                  && r.IsAdmin == requestMock.IsAdmin)), Times.Once);
        }

        #endregion

        #region DeleteUserAsync

        [TestMethod]
        public async Task DeleteUserAsync_WhenCalled_ShouldReturnOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var responseMock = Response.CreateOkResponse();
            _deleteUserUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<DeleteUserRequest>()))
                .ReturnsAsync(responseMock);
            
            // Act
            var result = await _controller.DeleteUserAsync(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);
            AssertHttpResponse(resultValue, HttpStatusCode.OK);
            _deleteUserUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<DeleteUserRequest>(
                r => r.Id == userId)), Times.Once);
        }

        #endregion
    }
}
