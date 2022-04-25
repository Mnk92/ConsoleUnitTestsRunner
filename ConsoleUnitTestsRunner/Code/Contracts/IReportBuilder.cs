using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code.Contracts
{
    internal interface IReportBuilder
    {
        void GenerateReport(string path, string xmlReport, ITestsMetricsCalculator tmc, IList<ResultMessage> testResults, double totalTimeInSeconds);
    }
}