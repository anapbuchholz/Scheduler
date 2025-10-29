using Moq;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql;
using System;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository
{
    [TestClass]
    public class UserRepositoryTests
    {
        private readonly Mock<ISqlHelper> _sqlHelperMock = null!;
        private readonly UserRepository _userRepository = null!;

        public UserRepositoryTests()
        {
            _sqlHelperMock = new Mock<ISqlHelper>();
            _userRepository = new UserRepository(_sqlHelperMock.Object);
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_WhenUserExists_ReturnsUserEntity()
        {
            // Arrange
            var email = "user@example.com";
            var expected = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = "Test User",
                DocumentNumber = "123",
                PasswordHash = "hash",
                ExternalId = "ext-1",
                IsAdmin = false,
                CompanyId = null
            };

            _sqlHelperMock
                .Setup(x => x.SelectFirstOrDefaultAsync<UserEntity>(UserSqlConstants.SELECT_USER_BY_EMAIL, It.IsAny<object>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _userRepository.GetUserByEmailAsync(email);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Email, result!.Email);
            Assert.AreEqual(expected.Id, result.Id);

            _sqlHelperMock.Verify(x =>
                x.SelectFirstOrDefaultAsync<UserEntity>(
                    UserSqlConstants.SELECT_USER_BY_EMAIL,
                    It.Is<object>(p => p.GetType().GetProperty("Email").GetValue(p).ToString() == email)
                ), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_WhenUserNotFound_ReturnsNull()
        {
            // Arrange
            var email = "notfound@example.com";

            _sqlHelperMock
                .Setup(x => x.SelectFirstOrDefaultAsync<UserEntity>(UserSqlConstants.SELECT_USER_BY_EMAIL, It.IsAny<object>()))
                .ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userRepository.GetUserByEmailAsync(email);

            // Assert
            Assert.IsNull(result);

            _sqlHelperMock.Verify(x =>
                x.SelectFirstOrDefaultAsync<UserEntity>(
                    UserSqlConstants.SELECT_USER_BY_EMAIL,
                    It.Is<object>(p => p.GetType().GetProperty("Email").GetValue(p).ToString() == email)
                ), Times.Once);
        }

        [TestMethod]
        public async Task RegisterUserAsync_CallsExecuteAsync_WithInsertCommandAndUser()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = "newuser@example.com",
                Name = "New User",
                DocumentNumber = "999",
                PasswordHash = "pwdhash",
                ExternalId = "ext-999",
                IsAdmin = false,
                CompanyId = Guid.NewGuid()
            };

            _sqlHelperMock
                .Setup(x => x.ExecuteAsync(UserSqlConstants.INSERT_USER, It.IsAny<object>()))
                .ReturnsAsync(1);

            // Act
            await _userRepository.RegisterUserAsync(user);

            // Assert
            _sqlHelperMock.Verify(x =>
                x.ExecuteAsync(
                    UserSqlConstants.INSERT_USER,
                    It.Is<UserEntity>(u => u.Email == user.Email && u.Id == user.Id)
                ), Times.Once);
        }
    }
}