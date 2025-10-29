using Scheduler.Application.Infrastructure.Configuration;
using System;

namespace Scheduler.UnitTests.Application.Infrastructure.Configuration
{
    [TestClass]
    public class EnvironmentVariableHandlerTests
    {
        private const string AspNetEnvName = "ASPNETCORE_ENVIRONMENT";
        private const string TestVarName = "SCHEDULER_TEST_VAR";

        [TestMethod]
        public void GetEnvironmentVariable_ReturnsValue_WhenSet()
        {
            var original = Environment.GetEnvironmentVariable(TestVarName, EnvironmentVariableTarget.Process);
            try
            {
                Environment.SetEnvironmentVariable(TestVarName, "my-value", EnvironmentVariableTarget.Process);

                var value = EnvironmentVariableHandler.GetEnvironmentVariable(TestVarName);

                Assert.AreEqual("my-value", value);
            }
            finally
            {
                Environment.SetEnvironmentVariable(TestVarName, original, EnvironmentVariableTarget.Process);
            }
        }

        [TestMethod]
        public void GetEnvironmentVariable_Throws_WhenNotSet()
        {
            var original = Environment.GetEnvironmentVariable(TestVarName, EnvironmentVariableTarget.Process);
            try
            {
                Environment.SetEnvironmentVariable(TestVarName, null, EnvironmentVariableTarget.Process);

                var ex = Assert.Throws<InvalidOperationException>(() =>
                    EnvironmentVariableHandler.GetEnvironmentVariable(TestVarName));

                Assert.Contains(TestVarName, ex.Message);
            }
            finally
            {
                Environment.SetEnvironmentVariable(TestVarName, original, EnvironmentVariableTarget.Process);
            }
        }

        [TestMethod]
        public void GetEnvironmentVariable_Throws_WhenEmpty()
        {
            var original = Environment.GetEnvironmentVariable(TestVarName, EnvironmentVariableTarget.Process);
            try
            {
                Environment.SetEnvironmentVariable(TestVarName, string.Empty, EnvironmentVariableTarget.Process);

                var ex = Assert.Throws<InvalidOperationException>(() =>
                    EnvironmentVariableHandler.GetEnvironmentVariable(TestVarName));

                Assert.Contains(TestVarName, ex.Message);
            }
            finally
            {
                Environment.SetEnvironmentVariable(TestVarName, original, EnvironmentVariableTarget.Process);
            }
        }

        [TestMethod]
        public void GetEnvironmentVariable_ReturnsWhitespace_WhenWhitespaceSet()
        {
            var original = Environment.GetEnvironmentVariable(TestVarName, EnvironmentVariableTarget.Process);
            try
            {
                Environment.SetEnvironmentVariable(TestVarName, "   ", EnvironmentVariableTarget.Process);

                var value = EnvironmentVariableHandler.GetEnvironmentVariable(TestVarName);

                Assert.AreEqual("   ", value);
            }
            finally
            {
                Environment.SetEnvironmentVariable(TestVarName, original, EnvironmentVariableTarget.Process);
            }
        }

        [TestMethod]
        public void IsDevelopment_ReturnsTrue_WhenDevelopment()
        {
            var original = Environment.GetEnvironmentVariable(AspNetEnvName, EnvironmentVariableTarget.Process);
            try
            {
                Environment.SetEnvironmentVariable(AspNetEnvName, "Development", EnvironmentVariableTarget.Process);

                Assert.IsTrue(EnvironmentVariableHandler.IsDevelopment());
            }
            finally
            {
                Environment.SetEnvironmentVariable(AspNetEnvName, original, EnvironmentVariableTarget.Process);
            }
        }

        [TestMethod]
        public void IsDevelopment_ReturnsFalse_WhenNotSet()
        {
            var original = Environment.GetEnvironmentVariable(AspNetEnvName, EnvironmentVariableTarget.Process);
            try
            {
                Environment.SetEnvironmentVariable(AspNetEnvName, null, EnvironmentVariableTarget.Process);

                Assert.IsFalse(EnvironmentVariableHandler.IsDevelopment());
            }
            finally
            {
                Environment.SetEnvironmentVariable(AspNetEnvName, original, EnvironmentVariableTarget.Process);
            }
        }

        [TestMethod]
        public void IsDevelopment_ReturnsFalse_ForDifferentValue_CaseSensitive()
        {
            var original = Environment.GetEnvironmentVariable(AspNetEnvName, EnvironmentVariableTarget.Process);
            try
            {
                Environment.SetEnvironmentVariable(AspNetEnvName, "development", EnvironmentVariableTarget.Process);

                Assert.IsFalse(EnvironmentVariableHandler.IsDevelopment());
            }
            finally
            {
                Environment.SetEnvironmentVariable(AspNetEnvName, original, EnvironmentVariableTarget.Process);
            }
        }
    }
}