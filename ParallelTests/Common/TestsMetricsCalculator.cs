using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelTests.Common
{
    public class TestsMetricsCalculator : ITestsMetricsCalculator
    {
        public int Errors { get; }
        public int Failures { get; }
        public int Skipped { get; }
        public int Invalid { get; }
        public int Inconclusive { get; }
        public int Ignored { get; }
        public int Passed { get; }
        public int Total { get; }
        public ResultMessage[] Failed { get; set; }
        public ResultMessage[] NotRun { get; set; }
        public IList<ResultMessage> Tests { get; }
        public IList<ResultMessage> All { get; }
        public int FailedCount => Failed.Length;

        public TestsMetricsCalculator(IEnumerable<ResultMessage> items, bool skipChildrenOnCalculateTests = false)
        {
            All = skipChildrenOnCalculateTests ? items.ToArray() : items.SelectMany(x => x.Collect()).ToArray();
            Tests = All.Where(x => x.IsTest()).ToArray();

            Failed = Tests.Where(x => x.IsFailed()).ToArray();
            NotRun = Tests.Where(x => x.State == ResultMessage.Types.TestResultState.Ignored ||
                                      x.State == ResultMessage.Types.TestResultState.Skipped).ToArray();

            Errors = Failed.Count(x => x.IsError());
            Failures = Failed.Count(x => x.IsFailure());

            Ignored = NotRun.Count(x => x.State == ResultMessage.Types.TestResultState.Ignored);
            Skipped = NotRun.Count(x => x.State == ResultMessage.Types.TestResultState.Skipped);

            Invalid = Tests.Count(x => x.State == ResultMessage.Types.TestResultState.NotRunnable);
            Inconclusive = Tests.Count(x => x.IsInconclusive());

            Total = Tests.Count;
            Passed = Total - Ignored - Inconclusive - Skipped - Invalid - Failures - Errors;
        }
    }

}
