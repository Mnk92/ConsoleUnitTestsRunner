using Mnk.ConsoleUnitTestsRunner.Code.Contracts;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    internal class ConsoleView : IConsoleView
    {
        private readonly DateTimeOffset startTime = DateTimeOffset.UtcNow;
        private readonly IReportBuilder reportBuilder;
        private readonly ITestsSummaryBuilder testsSummaryBuilder;
        public TestsResults TotalResult { get; private set; }
        private readonly List<TestsResults> results;
        private readonly object sync = new();
        private double totalTimeInSeconds;

        public ConsoleView(IReportBuilder reportBuilder, ITestsSummaryBuilder testsSummaryBuilder)
        {
            this.reportBuilder = reportBuilder;
            this.testsSummaryBuilder = testsSummaryBuilder;
            results = new List<TestsResults>();
        }

        public void AddResult(TestsResults result)
        {
            lock (sync)
            {
                results.Add(result);
            }
        }

        public void GenerateTotalResults()
        {
            // When the results are generated, then unit testing is done
            // => Also calculate total time taken.
            // Since tests might have run in parallel, simply adding the times
            // of the test results doesn't necessarily reflect the actual time taken.
            totalTimeInSeconds = (DateTimeOffset.UtcNow - startTime).TotalSeconds;

            lock (sync)
            {
                TotalResult = new TestsResults(results.SelectMany(x => x.Items).ToArray());
            }
        }

        public void PrintTotalResults()
        {
            Console.WriteLine(testsSummaryBuilder.Build(TotalResult.Metrics, totalTimeInSeconds));
        }

        public void GenerateXmlReport(string path, string xmlReport)
        {
            reportBuilder.GenerateReport(path, xmlReport, TotalResult.Metrics, TotalResult.Items, totalTimeInSeconds);
        }

    }
}
