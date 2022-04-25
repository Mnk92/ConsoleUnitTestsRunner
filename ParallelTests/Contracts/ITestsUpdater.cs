namespace Mnk.ParallelTests.Contracts
{
    public interface ITestsUpdater
    {
        void Update(int allCount, ResultMessage[] items, int failed, ISynchronizer synchronizer, TestsConfig config);
        void Update(string text);
        bool UserPressClose { get; }
    }
}
