using AutoFixture;
using Moq;
using Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.Company.GetCompany
{
    [TestClass]
    public class GetCompanyUseCaseTests
    {
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly GetCompanyUseCase _useCase;
        private readonly Fixture _fixture;

        public GetCompanyUseCaseTests()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _userSessionMock = new Mock<IUserSession>();
            _useCase = new GetCompanyUseCase(_companyRepositoryMock.Object, _userSessionMock.Object);
            _fixture = new Fixture();
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsNotAdmin_ReturnsForbidden()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(false);
            var request = new GetCompanyRequest(Guid.NewGuid());

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.AreEqual("Falta de permissão para realizar essa ação", response.ValidationErrorMessage);
            Assert.IsNull(response.Body);
            _companyRepositoryMock.Verify(x => x.GetCompanyAsync(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenCompanyNotFound_ReturnsNotFound()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var companyId = Guid.NewGuid();
            _companyRepositoryMock.Setup(x => x.GetCompanyAsync(companyId)).ReturnsAsync((CompanyEntity)null);
            var request = new GetCompanyRequest(companyId);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsNull(response.Body);
            _companyRepositoryMock.Verify(x => x.GetCompanyAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenCompanyFound_ReturnsOkWithCompany()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var companyId = Guid.NewGuid();
            var company = _fixture.Build<CompanyEntity>().With(c => c.Id, companyId).Create();
            _companyRepositoryMock.Setup(x => x.GetCompanyAsync(companyId)).ReturnsAsync(company);
            var request = new GetCompanyRequest(companyId);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(company, response.Body);
            _companyRepositoryMock.Verify(x => x.GetCompanyAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenExceptionThrown_ReturnsInternalError()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var companyId = Guid.NewGuid();
            _companyRepositoryMock.Setup(x => x.GetCompanyAsync(companyId)).ThrowsAsync(new Exception("DB error"));
            var request = new GetCompanyRequest(companyId);

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(response.ValidationErrorMessage.Contains("GetCompanyUseCase->ExecuteAsync"));
            Assert.IsTrue(response.ValidationErrorMessage.Contains("DB error"));
        }
    }
}
