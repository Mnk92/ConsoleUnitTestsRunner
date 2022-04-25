namespace Mnk.ParallelTests.Contracts
{
    public interface ITestsFixture : IDisposable
    {
        bool EnsurePathIsValid(TestsConfig config);
        TestsResults Refresh(TestsConfig config);
        TestsResults Run(TestsConfig config, TestsResults tests, ITestsUpdater updater, IList<ResultMessage> checkedTests = null);
    }
}
