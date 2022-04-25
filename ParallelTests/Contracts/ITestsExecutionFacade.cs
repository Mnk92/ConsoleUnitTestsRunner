using Mnk.Library.Interprocess;

namespace Mnk.ParallelTests.Contracts
{
    public interface ITestsExecutionFacade
    {
        TestsResults CollectTests(TestsConfig config, InterprocessClient<ITestsClient.ITestsClientClient> server);
        void Run(TestsConfig config, string handle);
    }
}