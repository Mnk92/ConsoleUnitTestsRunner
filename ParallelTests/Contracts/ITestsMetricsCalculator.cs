namespace Mnk.ParallelTests.Contracts
{
    public interface ITestsMetricsCalculator
    {
        int Errors { get; }
        int Failures { get; }
        int Skipped { get; }
        int Invalid { get; }
        int Inconclusive { get; }
        int Ignored { get; }
        int Passed { get; }
        int Total { get; }
        ResultMessage[] Failed { get; set; }
        ResultMessage[] NotRun { get; set; }
        IList<ResultMessage> Tests { get; }
        IList<ResultMessage> All { get; }
        int FailedCount { get; }
    }
}