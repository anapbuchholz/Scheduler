using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Scheduler.API.Controllers;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
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
        private readonly Fixture _fixture;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _registerUserUseCaseMock = new Mock<IUseCase<RegisterUserRequest, Response>>();
            _loginUseCaseMock = new Mock<IUseCase<LoginRequest, Response>>();
            _fixture = new Fixture();

            _controller = new UsersController(
                _registerUserUseCaseMock.Object,
                _loginUseCaseMock.Object
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
    }
}
