using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Scheduler.Application.Infrastructure.Authentication.Attributes;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Scheduler.UnitTests.Application.Infrastructure.Authentication.Attributes
{
    [TestClass]
    public class CustomAuthenticationHandlerAttributeTests
    {        
        [TestMethod]
        public async Task HandleAuthenticateAsync_NoAuthorizationHeader_ReturnsNoResult()
        {
            var userSessionMock = new Mock<IUserSessionSet>();
            var handler = CreateHandlerInstance(userSessionMock);

            var context = new DefaultHttpContext();
            // no Authorization header set

            await InitializeHandler(handler, context);

            var result = await InvokeHandleAuthenticateAsync(handler);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.None);
        }

        [TestMethod]
        public async Task HandleAuthenticateAsync_HeaderNotBearer_ReturnsFailWithInvalidMessage()
        {
            var userSessionMock = new Mock<IUserSessionSet>();
            var handler = CreateHandlerInstance(userSessionMock);

            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = "Token abc";

            await InitializeHandler(handler, context);

            var result = await InvokeHandleAuthenticateAsync(handler);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Failure);
            Assert.Contains("Invalid Authorization token", result.Failure.Message);
        }

        [TestMethod]
        public void ToClaims_SetsUserSessionAndReturnsClaims()
        {
            var userSessionMock = new Mock<IUserSessionSet>();
            var handler = CreateHandlerInstance(userSessionMock);

            // Prepare a claims dictionary matching expected keys
            var claims = new Dictionary<string, object>
            {
                { "email", "user@example.com" },
                { "user_id", "external-123" },
                { "name", "John Doe - 1" } // name and permission
            };

            // Invoke private ToClaims method via reflection
            var toClaimsMethod = handler.GetType().GetMethod("ToClaims", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(toClaimsMethod, "ToClaims method not found via reflection");

            var result = toClaimsMethod.Invoke(handler, [claims as IReadOnlyDictionary<string, object>, "Bearer abc"]) as IEnumerable<Claim>;
            Assert.IsNotNull(result);

            // Verify session was set correctly
            userSessionMock.Verify(x => x.SetUserSession("external-123", "John Doe", "user@example.com", true), Times.Once);

            // Verify returned claims contain expected entries
            var claimList = new List<Claim>(result);
            Assert.IsTrue(claimList.Exists(c => c.Type == "id" && c.Value == "external-123"));
            Assert.IsTrue(claimList.Exists(c => c.Type == "email" && c.Value == "user@example.com"));
            Assert.IsTrue(claimList.Exists(c => c.Type == "name" && c.Value == "John Doe"));
        }

        [TestMethod]
        public void ToClaims_Throws_WhenNameDoesNotContainPermissionPart()
        {
            var userSessionMock = new Mock<IUserSessionSet>();
            var handler = CreateHandlerInstance(userSessionMock);

            var claims = new Dictionary<string, object>
            {
                { "email", "user@example.com" },
                { "user_id", "external-123" },
                { "name", "InvalidFormatNameWithoutDash" }
            };

            var toClaimsMethod = handler.GetType().GetMethod("ToClaims", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(toClaimsMethod, "ToClaims method not found via reflection");

            // Expect exception because code parses permission part without validation
            Assert.Throws<TargetInvocationException>(() =>
            {
                // TargetInvocationException wraps the actual exception thrown inside the invoked method
                toClaimsMethod.Invoke(handler, [claims as IReadOnlyDictionary<string, object>, "Bearer abc"]);
            });
        }

        private static Mock<IOptionsMonitor<AuthenticationSchemeOptions>> CreateOptionsMonitorMock()
        {
            var mock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            mock.Setup(m => m.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());
            return mock;
        }

        private static async Task<AuthenticateResult> InvokeHandleAuthenticateAsync(object handlerInstance)
        {
            var method = handlerInstance.GetType().GetMethod("HandleAuthenticateAsync", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, "HandleAuthenticateAsync method not found via reflection");

            dynamic task = method.Invoke(handlerInstance, []);
            AuthenticateResult result = await task;
            return result;
        }

        private static async Task InitializeHandler(object handlerInstance, HttpContext context)
        {
            var scheme = new AuthenticationScheme("TestScheme", "TestDisplay", handlerInstance.GetType());
            var initMethod = handlerInstance.GetType().GetMethod("InitializeAsync", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(initMethod, "InitializeAsync method not found via reflection");
            dynamic task = initMethod.Invoke(handlerInstance, [scheme, context]);
            await task;
        }

        private static object CreateHandlerInstance(Mock<IUserSessionSet> userSessionMock, FirebaseApp firebaseApp = null)
        {
            var optionsMock = CreateOptionsMonitorMock();
            var loggerFactory = NullLoggerFactory.Instance;
            var encoder = UrlEncoder.Default;
            var clock = new SystemClock();

            // The class uses a primary-constructor; instantiate via normal new
            var ctor = typeof(CustomAuthenticationHandlerAttribute).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0];
            var instance = ctor.Invoke([optionsMock.Object, loggerFactory, encoder, clock, firebaseApp, userSessionMock.Object]);
            return instance;
        }
    }
}