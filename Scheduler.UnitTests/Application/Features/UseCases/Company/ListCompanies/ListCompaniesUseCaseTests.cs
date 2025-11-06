using AutoFixture;
using Dapper;
using Moq;
using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using System;
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
            _companyRepositoryMock.Verify(x => x.ListCompaniesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PaginationInput>()), Times.Never);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsAdmin_ReturnsOkWithCompanies()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<ListCompaniesRequest>();
            var totalCount = 3;
            var companies = _fixture.CreateMany<CompanyEntity>(totalCount);
            _companyRepositoryMock
                .Setup(x => x.ListCompaniesAsync(request.Name, request.DocumentNumber, request.PaginationParameters))
                .ReturnsAsync(new PaginatedQueryResult<CompanyEntity>(companies.AsList(), totalCount));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNull(response.ValidationErrorMessage);
            Assert.IsInstanceOfType(response.Body, typeof(PaginatedQueryResult<CompanyEntity>));
            var resultCompanies = response.Body as PaginatedQueryResult<CompanyEntity>;
            Assert.IsNotNull(resultCompanies);
            Assert.AreEqual(3, resultCompanies.TotalCount);
            _companyRepositoryMock.Verify(x => x.ListCompaniesAsync(
                It.Is<string>(r => r == request.Name),
                It.Is<string>(r => r == request.DocumentNumber),
                It.Is<PaginationInput>(i => i.PageNumber == request.PaginationParameters.PageNumber && i.PageSize == request.PaginationParameters.PageSize))
            , Times.Once);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenRepositoryThrowsException_ReturnsInternalError()
        {
            // Arrange
            _userSessionMock.Setup(x => x.IsAdmin).Returns(true);
            var request = _fixture.Create<ListCompaniesRequest>();
            _companyRepositoryMock
                .Setup(x => x.ListCompaniesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PaginationInput>()))
                .ThrowsAsync(new Exception("Repository error"));

            // Act
            var response = await _useCase.ExecuteAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("ListCompaniesUseCase->ExecuteAsync", response.ValidationErrorMessage);
            Assert.Contains("Repository error", response.ValidationErrorMessage);
        }
    }
}