using Mnk.ParallelTests.Common;

namespace Mnk.ParallelTests.Contracts
{
    public class TestsResults
    {
        public Exception Exception { get; private set; }
        public bool IsFailed => Exception != null;
        public ITestsMetricsCalculator Metrics { get; private set; }
        public IList<ResultMessage> Items { get; private set; }

        public TestsResults(IList<ResultMessage> items, bool skipChildrenOnCalculateTests = false, Exception ex = null)
        {
            Metrics = new TestsMetricsCalculator(items, skipChildrenOnCalculateTests);
            Items = items;
            Exception = ex;
        }

        public TestsResults(Exception ex = null) : this(Array.Empty<ResultMessage>(), false, ex)
        {
        }
    }
}
