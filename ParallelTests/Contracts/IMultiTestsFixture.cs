namespace Mnk.ParallelTests.Contracts
{
    public interface IMultiTestsFixture
    {
        IList<TestsExecutionContext> Refresh(IEnumerable<TestsConfig> configs, int assembliesInParallel);
        void Run(IEnumerable<TestsConfig> configs, int assembliesInParallel, IList<TestsExecutionContext> executionContexts, ITestsUpdater updater, Action<TestsExecutionContext> onEnd = null, IList<ResultMessage> checkedTests = null);
    }
}
