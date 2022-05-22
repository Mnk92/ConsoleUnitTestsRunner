using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Packages.Execution
{
    abstract class MultiProcessTestsExecutionFacade : ProcessTestsExecutionFacade
    {
        public override void Run(TestsConfig config, string handle)
        {
            Parallel.For(0, config.ProcessCount, _ =>
                Execute(() =>
                    Create(config, handle,
                        config.NeedSynchronizationForTests ? TestsCommands.Test : TestsCommands.FastTest)));
        }
    }
}
