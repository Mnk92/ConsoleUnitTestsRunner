using LightInject;
using Mnk.Library.Common.MT;
using Mnk.ParallelTests.Common;
using Mnk.ParallelTests.Contracts;
using NUnit.Framework;

namespace Mnk.ParallelNUnit.Tests.ParallelNUnit
{
    [TestFixture]
    [Category("Integration")]
    class SingleProcessNUnitTestFixture
    {
        private static readonly string TestsDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mnk.ParallelNUnit.Tests.dll");
        private const string NUnitAgentPath = "NUnitAgent.exe";
        private const string RunAsx86Path = "RunAsx86.exe";
        public static readonly bool[] Booleans = { true, false };
        public static readonly string[] Frameworks = { "net6" };
        private IServiceContainer container;
        private TestsConfig config;
        private ITestsUpdater updater;
        private ITestsFixture testsFixture;

        [SetUp]
        public void SetUp()
        {
            config = new TestsConfig
            {
                NUnitAgentPath = NUnitAgentPath,
                RunAsx86Path = RunAsx86Path,
                DirToCloneTests = Path.GetTempPath(),
                RunAsAdmin = false,
                Mode = TestsRunnerMode.Process
            };
            updater = new SimpleUpdater(new ConsoleUpdater());
            container = ServicesRegistrar.Register();
            testsFixture = container.GetInstance<ITestsFixture>();

        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void When_check_invalid_path([ValueSource(nameof(Booleans))] bool x86)
        {
            //Arrange
            config.TestDllPath = "Wrong path";
            config.RunAsx86 = x86;

            //Assert
            Assert.IsFalse(testsFixture.EnsurePathIsValid(config));
        }

        [Test]
        public void When_check_valid_path([ValueSource(nameof(Booleans))] bool x86)
        {
            //Arrange
            config.TestDllPath = TestsDllPath;
            config.RunAsx86 = x86;

            //Assert
            Assert.IsTrue(testsFixture.EnsurePathIsValid(config));
        }

        [Test]
        public void When_calc_tests_should_be_no_errors([ValueSource(nameof(Booleans))] bool x86)
        {
            //Arrange
            config.TestDllPath = TestsDllPath;
            config.RunAsx86 = x86;
            testsFixture.EnsurePathIsValid(config);

            //Act
            var results = testsFixture.Refresh(config);

            //Assert
            Assert.IsFalse(results.IsFailed);
        }

        [Test]
        public void When_calc_tests_should_calc_them([ValueSource(nameof(Booleans))] bool x86)
        {
            //Arrange
            config.TestDllPath = TestsDllPath;
            config.RunAsx86 = x86;
            testsFixture.EnsurePathIsValid(config);

            //Act
            var results = testsFixture.Refresh(config);

            //Assert
            Assert.Greater(results.Metrics.Total, 20);
        }

        [Test]
        [Pairwise]
        public void When_run_tests(
            [ValueSource(nameof(Booleans))] bool x86,
            [Values(1, 2)] int coresCount,
            [ValueSource(nameof(Booleans))] bool sync,
            [ValueSource(nameof(Booleans))] bool copy,
            [ValueSource(nameof(Booleans))] bool needOutput,
            [ValueSource(nameof(Booleans))] bool prefetch,
            [ValueSource(nameof(Frameworks))] string framework,
            [Values(0, 1)] int startDelay)
        {
            //Arrange
            config.TestDllPath = TestsDllPath;
            config.RunAsx86 = x86;
            config.RuntimeFramework = framework;
            config.ProcessCount = coresCount;
            config.Categories = new[] { "Integration" };
            config.IncludeCategories = false;
            config.OptimizeOrder = prefetch;
            config.CopyToSeparateFolders = copy;
            config.CopyMasks = new[] { "*.dll;*.exe" };
            config.StartDelay = startDelay;
            config.NeedSynchronizationForTests = sync;
            config.NeedOutput = needOutput;

            testsFixture.EnsurePathIsValid(config);
            var results = testsFixture.Refresh(config);

            //Act
            results = testsFixture.Run(config, results, updater);

            //Assert
            Assert.Greater(results.Metrics.Total, 20);
            Assert.AreEqual(0, results.Metrics.FailedCount, CollectFailed(results));
        }

        private static string CollectFailed(TestsResults results)
        {
            return string.Join(Environment.NewLine,
                results.Metrics.All
                    .Where(x => x.IsTest() && x.Executed() && x.IsFailed())
                    .Select(x => x.Key + " ------ " + x.Message + " ------ " + x.StackTrace)
                );
        }

    }
}
