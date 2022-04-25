using Mnk.Library.Interprocess;

namespace Mnk.ParallelTests.Contracts
{
    public interface ITestsExecutor
    {
        TestsResults CollectTests(TestsConfig config, InterprocessClient<ITestsClient.ITestsClientClient> server);
        TestsResults Run(TestsConfig config,
            ITestsMetricsCalculator metrics,
            IList<ResultMessage> allTests,
            IList<IList<ResultMessage>> packages,
            InterprocessServer<ITestsClient.ITestsClientBase, ITestsClient.ITestsClientClient> server,
            ITestsUpdater updater);
    }
}