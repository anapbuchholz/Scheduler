using Scheduler.Application.Infrastructure.Authorization.Implementations;

namespace Scheduler.UnitTests.Application.Infrastructure.Authorization.Implementations
{
    [TestClass]
    public class UserSessionTests
    {
        [TestMethod]
        public void DefaultValues_AreEmptyAndNotAdmin()
        {
            var session = new UserSession();

            Assert.AreEqual(string.Empty, session.UserExternalId);
            Assert.AreEqual(string.Empty, session.UserName);
            Assert.AreEqual(string.Empty, session.UserEmail);
            Assert.IsFalse(session.IsAdmin);
        }

        [TestMethod]
        public void SetUserSession_SetsAllProperties()
        {
            var session = new UserSession();

            session.SetUserSession("id-1", "User One", "one@example.com", true);

            Assert.AreEqual("id-1", session.UserExternalId);
            Assert.AreEqual("User One", session.UserName);
            Assert.AreEqual("one@example.com", session.UserEmail);
            Assert.IsTrue(session.IsAdmin);
        }

        [TestMethod]
        public void SetUserSession_OverwritesPreviousValues()
        {
            var session = new UserSession();

            session.SetUserSession("id-1", "User One", "one@example.com", true);
            session.SetUserSession("id-2", "User Two", "two@example.com", false);

            Assert.AreEqual("id-2", session.UserExternalId);
            Assert.AreEqual("User Two", session.UserName);
            Assert.AreEqual("two@example.com", session.UserEmail);
            Assert.IsFalse(session.IsAdmin);
        }

        [TestMethod]
        public void SetUserSession_AllowsEmptyStringsAndFalseAdmin()
        {
            var session = new UserSession();

            session.SetUserSession(string.Empty, string.Empty, string.Empty, false);

            Assert.AreEqual(string.Empty, session.UserExternalId);
            Assert.AreEqual(string.Empty, session.UserName);
            Assert.AreEqual(string.Empty, session.UserEmail);
            Assert.IsFalse(session.IsAdmin);
        }

        [TestMethod]
        public void SetUserSession_AllowsWhitespaceValues()
        {
            var session = new UserSession();

            session.SetUserSession("  id  ", "  name  ", "  email@x  ", true);

            Assert.AreEqual("  id  ", session.UserExternalId);
            Assert.AreEqual("  name  ", session.UserName);
            Assert.AreEqual("  email@x  ", session.UserEmail);
            Assert.IsTrue(session.IsAdmin);
        }
    }
}