namespace Mnk.ParallelTests.Contracts
{
    public interface ITestsDivider
    {
        IList<IList<ResultMessage>> Divide(TestsConfig config, ITestsMetricsCalculator metrics, IList<ResultMessage> checkedTests);
    }
}
