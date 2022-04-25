using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code.Contracts
{
    internal interface IConsoleView
    {
        TestsResults TotalResult { get; }
        void AddResult(TestsResults result);
        void GenerateTotalResults();
        void PrintTotalResults();
        void GenerateXmlReport(string path, string xmlReport);
    }
}