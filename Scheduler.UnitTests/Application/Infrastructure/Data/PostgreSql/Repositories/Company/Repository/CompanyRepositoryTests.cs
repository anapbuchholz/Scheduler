using AutoFixture;
using Dapper;
using Moq;
using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository
{
    [TestClass]
    public class CompanyRepositoryTests
    {
        private readonly Mock<ISqlHelper> _sqlHelperMock = null!;
        private readonly CompanyRepository _companyRepository = null!;
        private readonly Fixture _fixture;
        private readonly PaginationInput _paginationInput;

        public CompanyRepositoryTests()
        {
            _sqlHelperMock = new Mock<ISqlHelper>();
            _companyRepository = new CompanyRepository(_sqlHelperMock.Object);
            _fixture = new Fixture();
            _paginationInput = PaginationInput.Create(1, 10, null);
        }

        [TestMethod]
        public async Task GetCompanyAsync_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var expectedCompany = new CompanyEntity { Id = companyId, TradeName = "Test Co." };

            _sqlHelperMock
                .Setup(s => s.SelectFirstOrDefaultAsync<CompanyEntity>(
                    CompanySqlConstants.SELECT_COMPANY_BY_ID,
                    It.Is<object>(p => (Guid)p.GetType().GetProperty("Id")!.GetValue(p)! == companyId)))
                .ReturnsAsync(expectedCompany);

            // Act
            var result = await _companyRepository.GetCompanyAsync(companyId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCompany, result);
            _sqlHelperMock.VerifyAll();
        }

        [TestMethod]
        public async Task GetCompanyAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var companyId = Guid.NewGuid();

            _sqlHelperMock
                .Setup(s => s.SelectFirstOrDefaultAsync<CompanyEntity>(
                    CompanySqlConstants.SELECT_COMPANY_BY_ID,
                    It.IsAny<object>()))
                .ReturnsAsync((CompanyEntity)null);

            // Act
            var result = await _companyRepository.GetCompanyAsync(companyId);

            // Assert
            Assert.IsNull(result);
            _sqlHelperMock.VerifyAll();
        }

        [TestMethod]
        public async Task GetCompanyByDocumentNumberAsync_ShouldReturnCompany_WhenFound()
        {
            // Arrange
            var document = "12345678000100";
            var expectedCompany = new CompanyEntity { Id = Guid.NewGuid(), DocumentNumber = document };

            _sqlHelperMock
                .Setup(s => s.SelectFirstOrDefaultAsync<CompanyEntity>(
                    CompanySqlConstants.SELECT_COMPANY_BY_DOCUMENT_NUMBER,
                    It.Is<object>(p => (string)p.GetType().GetProperty("DocumentNumber")!.GetValue(p)! == document)))
                .ReturnsAsync(expectedCompany);

            // Act
            var result = await _companyRepository.GetCompanyByDocumentNumberAsync(document);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCompany, result);
            _sqlHelperMock.VerifyAll();
        }

        [TestMethod]
        public async Task ListCompaniesAsync_ShouldReturnAll_WhenNoFilters()
        {
            // Arrange            
            var expectedResult = _fixture.Create<PaginatedQueryResult<CompanyEntity>>();

            _sqlHelperMock
                .Setup(s => s.SelectPaginated<CompanyEntity>(
                    It.IsAny<PaginationInput>(),                    
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<DynamicParameters>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _companyRepository.ListCompaniesAsync(null, null, _paginationInput);

            // Assert
            Assert.AreEqual(expectedResult.TotalCount, result.TotalCount);
            _sqlHelperMock.VerifyAll();
        }

        [TestMethod]
        public async Task ListCompaniesAsync_ShouldFilterByName()
        {
            // Arrange
            var name = "alpha";
            var expectedResult = _fixture.Create<PaginatedQueryResult<CompanyEntity>>();

            _sqlHelperMock
                .Setup(s => s.SelectPaginated<CompanyEntity>(
                    It.IsAny<PaginationInput>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(q => q.Contains("LOWER(trade_name) LIKE")),
                    It.IsAny<bool>(),
                    It.Is<DynamicParameters>(p => p.ParameterNames.Contains("Name"))))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _companyRepository.ListCompaniesAsync(name, null, _paginationInput);

            // Assert
            Assert.AreEqual(expectedResult.TotalCount, result.TotalCount);
            _sqlHelperMock.Verify(x => x.SelectPaginated<CompanyEntity>(
                It.Is<PaginationInput>(p => p.PageNumber == _paginationInput.PageNumber && p.PageSize == _paginationInput.PageSize),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(q => q.Contains("LOWER(trade_name) LIKE")),
                true,
                It.Is<DynamicParameters>(p => p.ParameterNames.Contains("Name") && p.Get<string>("Name").Contains(name))), Times.Once);
        }

        [TestMethod]
        public async Task ListCompaniesAsync_ShouldFilterByDocumentNumber()
        {
            // Arrange
            var expectedResult = _fixture.Create<PaginatedQueryResult<CompanyEntity>>();
            var documentNumber = "99887766554433";

            _sqlHelperMock
                .Setup(s => s.SelectPaginated<CompanyEntity>(
                    It.IsAny<PaginationInput>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(q => q.Contains("tax_id = @DocumentNumber")),
                    It.IsAny<bool>(),
                    It.Is<DynamicParameters>(p => p.ParameterNames.Contains("DocumentNumber"))))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _companyRepository.ListCompaniesAsync(null, documentNumber, _paginationInput);

            // Assert
            Assert.AreEqual(expectedResult.TotalCount, result.TotalCount);
            _sqlHelperMock.Verify(x => x.SelectPaginated<CompanyEntity>(
                It.Is<PaginationInput>(p => p.PageNumber == _paginationInput.PageNumber && p.PageSize == _paginationInput.PageSize),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<string>(q => q.Contains("tax_id = @DocumentNumber")),
                true,
                It.Is<DynamicParameters>(p => p.ParameterNames.Contains("DocumentNumber") && p.Get<string>("DocumentNumber") == documentNumber)), Times.Once);
        }

        [TestMethod]
        public async Task RegisterCompanyAsync_ShouldCallExecuteAsync()
        {
            // Arrange
            var company = new CompanyEntity { Id = Guid.NewGuid(), TradeName = "New Co." };

            // Act
            await _companyRepository.RegisterCompanyAsync(company);

            // Assert
            _sqlHelperMock.Verify(m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }
    }
}
