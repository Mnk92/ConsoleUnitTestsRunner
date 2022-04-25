namespace Mnk.ParallelTests.Contracts
{
    public interface IOrderOptimizationManager
    {
        IList<ResultMessage> Optimize(string path, IList<ResultMessage> tests);
        void SaveStatistic(string path, IList<ResultMessage> tests);
    }
}
