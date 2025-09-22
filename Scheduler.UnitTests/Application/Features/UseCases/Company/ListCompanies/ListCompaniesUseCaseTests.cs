using AutoFixture;
using Moq;
using Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.Company.ListCompanies
{
    [TestClass]
    public class ListCompaniesUseCaseTests
    {
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly Fixture _fixture;
        private readonly ListCompaniesUseCase _useCase;

        public ListCompaniesUseCaseTests()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _userSessionMock = new Mock<IUserSession>();
            _fixture = new Fixture();
            _useCase = new ListCompaniesUseCase(_companyRepositoryMock.Object, _userSessionMock.Object);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsNotAdmin_ReturnsForbidden()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(false);
            var request = _fixture.Create<ListCompaniesRequest>();

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.AreEqual("Falta de permissão para realizar essa ação", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _companyRepositoryMock.Verify(x => x.ListCompaniesAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsAdmin_ReturnsOkWithCompanies()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<ListCompaniesRequest>();
            var companies = _fixture.CreateMany<CompanyEntity>(3);
            _companyRepositoryMock
                .Setup(x => x.ListCompaniesAsync(request.Name, request.DocumentNumber))
                .ReturnsAsync(new List<CompanyEntity>(companies));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.IsInstanceOfType(response.Body, typeof(List<CompanyEntity>));
            var resultCompanies = response.Body as List<CompanyEntity>;
            Assert.IsNotNull(resultCompanies);
            Assert.AreEqual(3, resultCompanies.Count);
            _companyRepositoryMock.Verify(x => x.ListCompaniesAsync(request.Name, request.DocumentNumber), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenRepositoryThrowsException_ReturnsInternalError()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<ListCompaniesRequest>();
            _companyRepositoryMock
                .Setup(x => x.ListCompaniesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Repository error"));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(response.ValidationErrorMessage.Contains("ListCompaniesUseCase->ExecuteAsync"));
            Assert.IsTrue(response.ValidationErrorMessage.Contains("Repository error"));
        }
    }
}