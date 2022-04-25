namespace Mnk.ParallelTests.Contracts
{
    public interface ITestsSummaryBuilder
    {
        string Build(ITestsMetricsCalculator metrics, double totalTimeInSeconds);
    }
}
