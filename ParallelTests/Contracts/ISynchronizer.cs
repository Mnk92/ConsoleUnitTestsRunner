namespace Mnk.ParallelTests.Contracts
{
    public interface ISynchronizer
    {
        int Finished { get; }
        void ProcessNextAgent(TestsConfig config, string handle);
    }
}