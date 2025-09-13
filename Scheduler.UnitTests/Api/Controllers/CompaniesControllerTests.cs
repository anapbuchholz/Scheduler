using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Scheduler.API.Controllers;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase;
using Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Api.Controllers
{
    [TestClass]
    public sealed class CompaniesControllerTests
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
        public async Task ListCompaniesAsync_WhenCalled_ShouldReturnUseCaseResultHttpStatusCode()
        {
            //Arrange
            var requestMock = _fixture.Create<ListCompaniesRequest>();
            var responseBodyMock = _fixture.CreateMany<string>(3).ToList();
            var responseMock = Response.CreateOkResponse(responseBodyMock);

            _listCompanyUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<ListCompaniesRequest>()))
                .ReturnsAsync(responseMock);

            //Act
            var result = await _controller.ListCompaniesAsync(requestMock.Name, requestMock.DocumentNumber);
            
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var resultValue = result as OkObjectResult;
            Assert.IsNotNull(resultValue);

            var value = resultValue.Value;
            var bodyProperty = value.GetType().GetProperty("Body");
            var responseBody = bodyProperty?.GetValue(value) as List<string>;
            CollectionAssert.AreEqual(responseBodyMock, responseBody);

            _listCompanyUseCaseMock.Verify(x => x.ExecuteAsync(It.Is<ListCompaniesRequest>(
                r => r.DocumentNumber == requestMock.DocumentNumber 
                && r.Name == requestMock.Name))
            , Times.Once);
        }        

        #endregion        
    }
}