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
        public async Task GetUserByDodcumentNumberAsync_WhenUserExists_ReturnsUserEntity()
        {
            // Arrange
            var documentNumber = "11111111111";
            var expected = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = "test@email.com",
                Name = "Test User",
                DocumentNumber = documentNumber,
                PasswordHash = "hash",
                ExternalId = "ext-1",
                IsAdmin = false,
                CompanyId = null
            };

            _sqlHelperMock
                .Setup(x => x.SelectFirstOrDefaultAsync<UserEntity>(UserSqlConstants.SELECT_USER_BY_DOCUMENT_NUMBER, It.IsAny<object>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _userRepository.GetUserByDocumentNumberAsync(documentNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.DocumentNumber, result!.DocumentNumber);
            Assert.AreEqual(expected.Id, result.Id);

            _sqlHelperMock.Verify(x =>
                x.SelectFirstOrDefaultAsync<UserEntity>(
                    UserSqlConstants.SELECT_USER_BY_DOCUMENT_NUMBER,
                    It.Is<object>(p => p.GetType().GetProperty("DocumentNumber").GetValue(p).ToString() == documentNumber)
                ), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByDocumentNummberAsync_WhenUserNotFound_ReturnsNull()
        {
            // Arrange
            var documentNumber = "11111111111";

            _sqlHelperMock
                .Setup(x => x.SelectFirstOrDefaultAsync<UserEntity>(UserSqlConstants.SELECT_USER_BY_DOCUMENT_NUMBER, It.IsAny<object>()))
                .ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userRepository.GetUserByDocumentNumberAsync(documentNumber);

            // Assert
            Assert.IsNull(result);

            _sqlHelperMock.Verify(x =>
                x.SelectFirstOrDefaultAsync<UserEntity>(
                    UserSqlConstants.SELECT_USER_BY_DOCUMENT_NUMBER,
                    It.Is<object>(p => p.GetType().GetProperty("DocumentNumber").GetValue(p).ToString() == documentNumber)
                ), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUserEntity()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new UserEntity
            {
                Id = id,
                Email = "test@email.com",
                Name = "Test User",
                DocumentNumber = "11111111111",
                PasswordHash = "hash",
                ExternalId = "ext-1",
                IsAdmin = false,
                CompanyId = null
            };

            _sqlHelperMock
                .Setup(x => x.SelectFirstOrDefaultAsync<UserEntity>(UserSqlConstants.SELECT_USER_BY_ID, It.IsAny<object>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _userRepository.GetUserByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);;
            Assert.AreEqual(expected.Id, result.Id);

            _sqlHelperMock.Verify(x =>
                x.SelectFirstOrDefaultAsync<UserEntity>(
                    UserSqlConstants.SELECT_USER_BY_ID,
                    It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).ToString() == id.ToString())
                ), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            _sqlHelperMock
                .Setup(x => x.SelectFirstOrDefaultAsync<UserEntity>(UserSqlConstants.SELECT_USER_BY_ID, It.IsAny<object>()))
                .ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userRepository.GetUserByIdAsync(id);

            // Assert
            Assert.IsNull(result);

            _sqlHelperMock.Verify(x =>
                x.SelectFirstOrDefaultAsync<UserEntity>(
                    UserSqlConstants.SELECT_USER_BY_ID,
                    It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).ToString() == id.ToString())
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