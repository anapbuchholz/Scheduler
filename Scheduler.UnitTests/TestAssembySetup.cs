using SQLitePCL;

namespace Scheduler.UnitTests
{
    [TestClass]
    public static class TestAssemblySetup
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Batteries.Init();
        }
    }
}