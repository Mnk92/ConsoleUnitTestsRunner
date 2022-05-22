using LightInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mnk.Library.Common.MT;
using Mnk.ParallelTests.Common;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Tests.ParallelNUnit
{
    public class TestFixture
    {
        private static readonly string TestsDllPath = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                             $"../../../../NUnitTests.Example/bin/{Shared.CompileMode}/net6.0/NUnitTests.Example.dll")
            );
        public static readonly bool[] Booleans = { true, false };
        public static readonly string[] Frameworks = { "net6" };
        private IServiceContainer container;
        protected TestsConfig Config { get; set; }
        private ITestsUpdater updater;
        private ITestsFixture testsFixture;

        [TestInitialize]
        public virtual void SetUp()
        {
            updater = new SimpleUpdater(new ConsoleUpdater());
            container = ServicesRegistrar.Register();
            testsFixture = container.GetInstance<ITestsFixture>();
        }

        [TestCleanup]
        public void TearDown()
        {
            container.Dispose();
        }

        [TestMethod]
        public void When_check_invalid_path()
        {
            //Arrange
            Config.TestDllPath = "Wrong path";

            //Assert
            Assert.IsFalse(testsFixture.EnsurePathIsValid(Config));
        }

        [TestMethod]
        public void When_check_valid_path()
        {
            //Arrange
            Config.TestDllPath = TestsDllPath;

            //Assert
            Assert.IsTrue(testsFixture.EnsurePathIsValid(Config));
        }

        [TestMethod]
        public void When_calc_tests_should_be_no_errors()
        {
            //Arrange
            Config.TestDllPath = TestsDllPath;
            testsFixture.EnsurePathIsValid(Config);

            //Act
            var results = testsFixture.Refresh(Config);

            //Assert
            Assert.IsFalse(results.IsFailed);
        }

        [TestMethod]
        public void When_calc_tests_should_calc_them()
        {
            //Arrange
            Config.TestDllPath = TestsDllPath;
            testsFixture.EnsurePathIsValid(Config);

            //Act
            var results = testsFixture.Refresh(Config);

            //Assert
            Assert.IsTrue(results.Metrics.Total > 10);
        }

        [DataTestMethod]
        [DataRow(1, false, false, false, false, "net6.0", 0)]
        [DataRow(2, true, true, true, true, "net6.0", 1)]
        public void When_run_tests(int coresCount, bool sync, bool copy, bool needOutput, bool prefetch, string framework, int startDelay)
        {
            //Arrange
            Config.TestDllPath = TestsDllPath;
            Config.RuntimeFramework = framework;
            Config.ProcessCount = coresCount;
            Config.Categories = new[] { "Integration" };
            Config.IncludeCategories = false;
            Config.OptimizeOrder = prefetch;
            Config.CopyToSeparateFolders = copy;
            Config.CopyMasks = new[] { "*.dll;*.exe" };
            Config.StartDelay = startDelay;
            Config.NeedSynchronizationForTests = sync;
            Config.NeedOutput = needOutput;

            testsFixture.EnsurePathIsValid(Config);
            var results = testsFixture.Refresh(Config);

            //Act
            results = testsFixture.Run(Config, results, updater);

            //Assert
            Assert.IsTrue(results.Metrics.Total > 10);
            Assert.AreEqual(0, results.Metrics.FailedCount, CollectFailed(results));
        }

        private string CollectFailed(TestsResults results)
        {
            return string.Join(Environment.NewLine,
                results.Metrics.All
                    .Where(x => x.IsTest() && x.Executed() && x.IsFailed())
                    .Select(x => x.Key + " ------ " + x.Message + " ------ " + x.StackTrace)
                );
        }

    }
}
