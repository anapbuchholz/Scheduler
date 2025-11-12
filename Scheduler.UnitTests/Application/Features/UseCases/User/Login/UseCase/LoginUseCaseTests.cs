using AutoFixture;
using Moq;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.Login.UseCase
{
    [TestClass]
    public class LoginUseCaseTests
    {
        private readonly Mock<IRequestValidator<LoginRequest>> _validatorMock;
        private readonly Mock<IFireBaseAuthenticationService> _fireBaseAuthServiceMock;
        private readonly Fixture _fixture;
        private readonly LoginUseCase _useCase;

        public LoginUseCaseTests()
        {
            _validatorMock = new Mock<IRequestValidator<LoginRequest>>();
            _fireBaseAuthServiceMock = new Mock<IFireBaseAuthenticationService>();
            _fixture = new Fixture();
            _useCase = new LoginUseCase(_validatorMock.Object, _fireBaseAuthServiceMock.Object);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenValidationFails_ReturnsInvalidParameters()
        {
            // Arrange
            var request = _fixture.Create<LoginRequest>();
            var errors = new List<string> { "E-mail inválido" };
            var validationModel = new RequestValidationModel(errors);
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(validationModel.ErrorMessage, response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _fireBaseAuthServiceMock.Verify(x => x.LoginInFireBaseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenAuthenticationFails_ReturnsNotFound()
        {
            // Arrange
            var request = _fixture.Create<LoginRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _fireBaseAuthServiceMock
                .Setup(x => x.LoginInFireBaseAsync(request.Email!, request.Password!))
                .ReturnsAsync((false, null));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenAuthenticationSucceeds_ReturnsOkWithToken()
        {
            // Arrange
            var request = _fixture.Create<LoginRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            var token = Guid.NewGuid().ToString();
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _fireBaseAuthServiceMock
                .Setup(x => x.LoginInFireBaseAsync(request.Email!, request.Password!))
                .ReturnsAsync((true, token));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(response.Body);
            var bodyType = response.Body.GetType();
            var tokenProp = bodyType.GetProperty("Token");
            Assert.IsNotNull(tokenProp);
            Assert.AreEqual(token, tokenProp.GetValue(response.Body));
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenExceptionThrown_ReturnsInternalError()
        {
            // Arrange
            var request = _fixture.Create<LoginRequest>();
            var validationModel = new RequestValidationModel(new List<string>());
            _validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(validationModel);
            _fireBaseAuthServiceMock
                .Setup(x => x.LoginInFireBaseAsync(request.Email!, request.Password!))
                .ThrowsAsync(new Exception("Firebase error"));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(response.ValidationErrorMessage.Contains("LoginUseCase->ExecuteAsync"));
            Assert.IsTrue(response.ValidationErrorMessage.Contains("Firebase error"));
        }
    }
}