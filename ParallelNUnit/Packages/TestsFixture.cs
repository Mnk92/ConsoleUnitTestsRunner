using Mnk.Library.Common.Log;
using Mnk.Library.Interprocess;
using Mnk.ParallelTests.Contracts;
using Mnk.ParallelNUnit.Core;

namespace Mnk.ParallelNUnit.Packages
{
    class TestsFixture : ITestsFixture
    {
        private readonly IOrderOptimizationManager orderOptimizationManager;
        private InterprocessServer<ITestsClient.ITestsClientBase, ITestsClient.ITestsClientClient> Server { get; }
        private readonly ILog log = LogManager.GetLogger<TestsFixture>();
        private readonly ITestsExecutor testsExecutor;
        private readonly ITestsDivider testsDivider;

        public TestsFixture(IOrderOptimizationManager orderOptimizationManager, ITestsExecutor testsExecutor, ITestsDivider testsDivider)
        {
            this.orderOptimizationManager = orderOptimizationManager;
            this.testsExecutor = testsExecutor;
            this.testsDivider = testsDivider;
            Server = new InterprocessServer<ITestsClient.ITestsClientBase, ITestsClient.ITestsClientClient>(new TestsClient());
        }

        public bool EnsurePathIsValid(TestsConfig config)
        {
            if (File.Exists(config.TestDllPath)) return true;
            log.Write("Can't load dll: " + config.TestDllPath);
            return false;
        }

        public TestsResults Refresh(TestsConfig config)
        {
            try
            {
                ((TestsClient)Server.Instance).Prepare();
                return testsExecutor.CollectTests(config, Server.Client.Value);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't refresh tests from dll: " + config.TestDllPath);
                return new TestsResults(ex);
            }
        }

        public TestsResults Run(TestsConfig config, TestsResults tests, ITestsUpdater updater, IList<ResultMessage> checkedTests = null)
        {
            try
            {
                var divided = testsDivider.Divide(config, tests.Metrics, checkedTests);
                var r = testsExecutor.Run(config, tests.Metrics, tests.Items, divided, Server, updater);
                if (config.OptimizeOrder)
                {
                    orderOptimizationManager.SaveStatistic(config.TestDllPath, r.Metrics.Tests);
                }
                return r;
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't run test, from dll: " + config.TestDllPath);
                return new TestsResults(ex);
            }
        }
        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
