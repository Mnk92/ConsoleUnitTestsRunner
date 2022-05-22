using Mnk.Library.Interprocess;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Packages.Execution
{
    class InternalTestsExecutionFacade : ITestsExecutionFacade
    {
        private readonly ITestFacade executor;

        public InternalTestsExecutionFacade(ITestFacade executor)
        {
            this.executor = executor;
        }

        public TestsResults CollectTests(TestsConfig config, InterprocessClient<ITestsClient.ITestsClientClient> server)
        {
            var results = executor.CollectTests(config.TestDllPath, config);
            if (results == null)
                throw new ArgumentException("Can't collect tests in: " + config.TestDllPath);
            return new TestsResults(new[] { results });
        }

        public void Run(TestsConfig config, string handle)
        {
            executor.RunTests(config, handle);
        }
    }
}
