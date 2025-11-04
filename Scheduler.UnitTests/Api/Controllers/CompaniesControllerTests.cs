using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Scheduler.API.Controllers;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase;
using Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.UnitTests.Api.Controllers.Base;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Api.Controllers
{
    [TestClass]
    public sealed class CompaniesControllerTests : ControllerUnitTestBase
    {
        private readonly Mock<IUseCase<RegisterCompanyRequest, Response>> _registerCompanyUseCaseMock;
        private readonly Mock<IUseCase<GetCompanyRequest, Response>> _getCompanyUseCaseMock;
        private readonly Mock<IUseCase<ListCompaniesRequest, Response>> _listCompanyUseCaseMock;
        private readonly Fixture _fixture;

        private readonly CompaniesController _controller;

        public CompaniesControllerTests()
        {
            _registerCompanyUseCaseMock = new Mock<IUseCase<RegisterCompanyRequest, Response>>();
            _getCompanyUseCaseMock = new Mock<IUseCase<GetCompanyRequest, Response>>();
            _listCompanyUseCaseMock = new Mock<IUseCase<ListCompaniesRequest, Response>>();
            _fixture = new Fixture();

            _controller = new CompaniesController(
                _registerCompanyUseCaseMock.Object,
                _getCompanyUseCaseMock.Object,
                _listCompanyUseCaseMock.Object
            );
        }

        #region ListCompaniesAsync

        [TestMethod]
        public async Task ListCompaniesAsync_WhenCalled_ShouldReturnCompanyEntities()
        {
            //Arrange
            var requestMock = _fixture.Create<ListCompaniesRequest>();
            var responseBodyMock = _fixture.CreateMany<CompanyEntity>(3).ToList();
            var responseMock = Response.CreateOkResponse(responseBodyMock);            

            _listCompanyUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<ListCompaniesRequest>()))
                .ReturnsAsync(responseMock);

            //Act
            var result = await _controller.ListCompaniesAsync(requestMock.PaginationParameters.PageNumber, requestMock.PaginationParameters.PageSize, requestMock.Name, requestMock.DocumentNumber);

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);

            AssertHttpResponse(resultValue, HttpStatusCode.OK, responseBodyMock);

            _listCompanyUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<ListCompaniesRequest>(
                r => r.DocumentNumber == requestMock.DocumentNumber
                && r.Name == requestMock.Name)), Times.Once);
        }

        #endregion

        #region GetCompanyAsync

        [TestMethod]
        public async Task GetCompanyAsync_WhenCalled_ShouldReturnCompanyEntity()
        {
            // Arrange
            var idMock = _fixture.Create<Guid>();
            var responseBodyMock = _fixture.Create<CompanyEntity>();
            var responseMock = Response.CreateOkResponse(responseBodyMock);

            _getCompanyUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<GetCompanyRequest>()))
                .ReturnsAsync(responseMock);

            // Act
            var result = await _controller.GetCompanyAsync(idMock);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);

            AssertHttpResponse(resultValue, HttpStatusCode.OK, responseBodyMock);

            _getCompanyUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<GetCompanyRequest>(
                r => r.Id == idMock)), Times.Once);
        }

        #endregion

        #region RegisterCompanyAsync

        [TestMethod]
        public async Task RegisterCompanyAsync_WhenCalled_ShouldReturnCreatedCompanyEntity()
        {
            // Arrange
            var requestMock = _fixture.Create<RegisterCompanyRequest>();
            var responseMock = Response.CreateCreatedResponse();

            _registerCompanyUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<RegisterCompanyRequest>()))
                .ReturnsAsync(responseMock);

            // Act
            var result = await _controller.RegisterCompanyAsync(requestMock);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            var resultValue = result as CreatedResult;
            Assert.IsNotNull(resultValue);

            AssertHttpResponse(resultValue, HttpStatusCode.Created);

            _registerCompanyUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<RegisterCompanyRequest>(
                r => r.TradeName == requestMock.TradeName
                  && r.LegalName == requestMock.LegalName
                  && r.DocumentNumber == requestMock.DocumentNumber
                  && r.Email == requestMock.Email
                  && r.PhoneNumber == requestMock.PhoneNumber)), Times.Once);
        }

        #endregion
    }
}