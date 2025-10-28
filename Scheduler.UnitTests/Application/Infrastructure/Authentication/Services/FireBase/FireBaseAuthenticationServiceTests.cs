using FirebaseAdmin.Auth;
using Moq;
using Moq.Protected;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Implementations;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using Scheduler.UnitTests.Application.Infrastructure.Authentication.Services.FireBase;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Scheduler.UnitTests.Application.Infrastructure.Authentication.Services.FireBase
{
    [TestClass]
    public class FireBaseAuthenticationServiceTests
    {
        [TestMethod]
        public async Task LoginInFireBase_WhenResponseIsSuccess_ReturnsToken()
        {
            var expectedToken = "jwt-token-123";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new { IdToken = expectedToken })
            };

            var httpClient = CreateHttpClientReturning(response);
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            var (isAuthenticated, jwt) = await service.LoginInFireBase("u@x.com", "pwd");

            Assert.IsTrue(isAuthenticated);
            Assert.AreEqual(expectedToken, jwt);
        }

        [TestMethod]
        public async Task LoginInFireBase_WhenResponseIsNotSuccess_ReturnsFalseAndNull()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(new { error = "bad" })
            };

            var httpClient = CreateHttpClientReturning(response);
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            var (isAuthenticated, jwt) = await service.LoginInFireBase("u@x.com", "wrong");

            Assert.IsFalse(isAuthenticated);
            Assert.IsNull(jwt);
        }

        [TestMethod]
        public async Task RegisterFireBaseUserAsync_WhenCreateSucceeds_ReturnsUidAndTrue()
        {
            var httpClient = CreateHttpClientReturning(new HttpResponseMessage(HttpStatusCode.OK));
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            var fakeUser = CreateUserRecord("uid-123");
            proxyMock.Setup(p => p.CreateUserAsync(It.IsAny<UserRecordArgs>())).ReturnsAsync(fakeUser);

            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            var (externalId, registered) = await service.RegisterFireBaseUserAsync("a@b.com", "pwd", "display");

            Assert.IsTrue(registered);
            Assert.AreEqual("uid-123", externalId);
            proxyMock.Verify(p => p.CreateUserAsync(It.IsAny<UserRecordArgs>()), Times.Once);
        }

        [TestMethod]
        public async Task RegisterFireBaseUserAsync_WhenCreateThrows_ReturnsNullAndFalse()
        {
            var httpClient = CreateHttpClientReturning(new HttpResponseMessage(HttpStatusCode.OK));
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            proxyMock.Setup(p => p.CreateUserAsync(It.IsAny<UserRecordArgs>())).ThrowsAsync(new Exception("fail"));

            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            var (externalId, registered) = await service.RegisterFireBaseUserAsync("a@b.com", "pwd", "display");

            Assert.IsFalse(registered);
            Assert.IsNull(externalId);
            proxyMock.Verify(p => p.CreateUserAsync(It.IsAny<UserRecordArgs>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateFireBaseUser_CallsProxyUpdate()
        {
            var httpClient = CreateHttpClientReturning(new HttpResponseMessage(HttpStatusCode.OK));
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            proxyMock.Setup(p => p.UpdateUserAsync(It.IsAny<UserRecordArgs>())).ReturnsAsync(CreateUserRecord("any"));

            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            await service.UpdateFireBaseUser(new UserRecordArgs { Email = "x@y" });

            proxyMock.Verify(p => p.UpdateUserAsync(It.IsAny<UserRecordArgs>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteFireBaseUserAsync_WhenUserExists_DeletesAndReturnsTrue()
        {
            var httpClient = CreateHttpClientReturning(new HttpResponseMessage(HttpStatusCode.OK));
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            var fakeUser = CreateUserRecord("uid-to-del");
            proxyMock.Setup(p => p.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(fakeUser);
            proxyMock.Setup(p => p.DeleteUserAsync("uid-to-del")).Returns(Task.CompletedTask);

            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            var result = await service.DeleteFireBaseUserAsync("some@mail");

            Assert.IsTrue(result);
            proxyMock.Verify(p => p.GetUserByEmailAsync(It.IsAny<string>()), Times.Once);
            proxyMock.Verify(p => p.DeleteUserAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteFireBaseUserAsync_WhenGetUserThrows_ReturnsFalse()
        {
            var httpClient = CreateHttpClientReturning(new HttpResponseMessage(HttpStatusCode.OK));
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            proxyMock.Setup(p => p.GetUserByEmailAsync(It.IsAny<string>())).ThrowsAsync(new Exception("not found"));

            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            var result = await service.DeleteFireBaseUserAsync("missing@mail");

            Assert.IsFalse(result);
            proxyMock.Verify(p => p.GetUserByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetFireBaseUserByEmail_ReturnsUserRecord()
        {
            var httpClient = CreateHttpClientReturning(new HttpResponseMessage(HttpStatusCode.OK));
            var proxyMock = new Mock<IFireBaseAdminProxy>();
            var fakeUser = CreateUserRecord("uid-xyz");
            proxyMock.Setup(p => p.GetUserByEmailAsync("u@x")).ReturnsAsync(fakeUser);

            var service = new FireBaseAuthenticationService(httpClient, proxyMock.Object);

            var user = await service.GetFireBaseUserByEmail("u@x");

            Assert.IsNotNull(user);
            Assert.AreEqual("uid-xyz", user.Uid);
            proxyMock.Verify(p => p.GetUserByEmailAsync("u@x"), Times.Once);
        }

        private static HttpClient CreateHttpClientReturning(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();

            return new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost/") };
        }

        // Helper para criar instância de UserRecord sem chamar construtor público
        private static UserRecord CreateUserRecord(string uid)
        {
            // cria o objeto sem executar construtor
            var obj = (UserRecord)FormatterServices.GetUninitializedObject(typeof(UserRecord));

            // tenta setar propriedade Uid via setter não público (se existir)
            var prop = typeof(UserRecord).GetProperty("Uid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, uid);
                return obj;
            }

            // tenta localizar campo backing field comum de auto-propriedades
            var field = typeof(UserRecord).GetField("<Uid>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?? typeof(UserRecord).GetField("_uid", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?? typeof(UserRecord).GetField("uid", BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
            {
                field.SetValue(obj, uid);
                return obj;
            }

            // última tentativa: setar via qualquer campo que contenha "uid" no nome
            foreach (var f in typeof(UserRecord).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (f.Name.Contains("uid", StringComparison.OrdinalIgnoreCase))
                {
                    f.SetValue(obj, uid);
                    return obj;
                }
            }

            throw new NotSupportedException("Não foi possível inicializar UserRecord para testes (uid não encontrado).");
        }
    }
}