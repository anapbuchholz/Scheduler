using AutoFixture;
using Moq;
using Scheduler.Application.Features.UseCases.User.ListUsers.UseCase;
using Scheduler.Application.Features.UseCases.User.Shared;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Features.UseCases.User.ListUsers.UseCase
{
    [TestClass]
    public class ListUsersUseCaseTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserSession> _userSessionMock;
        private readonly ListUsersUseCase _listUsersUseCase;
        private readonly Fixture _fixture;

        public ListUsersUseCaseTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userSessionMock = new Mock<IUserSession>();
            _listUsersUseCase = new ListUsersUseCase(_userRepositoryMock.Object, _userSessionMock.Object);
            _fixture = new Fixture();
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsNotAdmin_ShouldREturnForbiddenResponse()
        {
            // Arrange
            _userSessionMock.Setup(us => us.IsAdmin).Returns(false);
            var request = _fixture.Create<ListUsersRequest>();

            // Act
            var response = await _listUsersUseCase.ExecuteAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_WhenUserIsAdmin_ShouldReturnOkResponse()
        {
            // Arrange
            _userSessionMock.Setup(us => us.IsAdmin).Returns(true);
            var request = _fixture.Create<ListUsersRequest>();
            var paginatedResult = _fixture.Create<PaginatedQueryResult<UserEntity>>();
            _userRepositoryMock.Setup(ur => ur.ListUsersAsync(request.Name, request.Email, request.DocumentNumber, request.IsAdmin, request.PaginationParameters))
                .ReturnsAsync(paginatedResult);
            var expectedResponse = new PaginatedQueryResult<GetUserResponse>(
                paginatedResult.Results.ConvertAll(user => new GetUserResponse(user)),
                paginatedResult.TotalCount);

            // Act
            var response = await _listUsersUseCase.ExecuteAsync(request);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(response.Body);
            var actualResponse = response.Body as PaginatedQueryResult<GetUserResponse>;
            Assert.AreEqual(expectedResponse.TotalCount, actualResponse.TotalCount);
        }
    }
}
