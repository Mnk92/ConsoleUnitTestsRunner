using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Interprocess;
using Mnk.ParallelNUnit.Core;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Packages.Common
{
    class TestsExecutor : ITestsExecutor
    {
        private readonly IDirectoriesManipulator directoriesManipulator;
        private readonly Func<string, ITestsExecutionFacade> factory;
        private readonly ILog Log = LogManager.GetLogger<TestsExecutor>();

        public TestsExecutor(IDirectoriesManipulator directoriesManipulator, Func<string, ITestsExecutionFacade> factory)
        {
            this.directoriesManipulator = directoriesManipulator;
            this.factory = factory;
        }

        public TestsResults CollectTests(TestsConfig config, InterprocessClient<ITestsClient.ITestsClientClient> server)
        {
            return GetFacade(config).CollectTests(config, server);
        }

        public TestsResults Run(TestsConfig config, ITestsMetricsCalculator metrics, IList<ResultMessage> allTests, IList<IList<ResultMessage>> packages, InterprocessServer<ITestsClient.ITestsClientBase, ITestsClient.ITestsClientClient> server, ITestsUpdater updater)
        {
            var dllPaths = directoriesManipulator.GenerateFolders(config, updater, packages.Count);
            try
            {
                if (updater.UserPressClose) return new TestsResults();
                var handle = server.Handle;
                if (!string.IsNullOrEmpty(config.CommandBeforeTestsRun))
                {
                    foreach (var folder in dllPaths)
                    {
                        Cmd.Start(config.CommandBeforeTestsRun, Log,
                            directory: Path.GetDirectoryName(folder),
                            waitEnd: true,
                            noWindow: true);
                        if (updater.UserPressClose) return new TestsResults();
                    }
                }
                if (updater.UserPressClose) return new TestsResults();
                ((TestsClient)server.Instance).PrepareToRun(new Synchronizer(), updater,
                    new TestRunConfigMessage
                    {
                        DllPaths = { dllPaths },
                        StartDelay = config.StartDelay * 1000,
                    },
                    packages, allTests,
                    metrics, config
                    );
                GetFacade(config).Run(config, handle);
            }
            finally
            {
                directoriesManipulator.ClearFolders(config, dllPaths);
            }
            return new TestsResults(allTests, config.SkipChildrenOnCalculateTests);
        }

        private ITestsExecutionFacade GetFacade(TestsConfig config)
        {
            return factory(config.Mode);
        }

    }
}
