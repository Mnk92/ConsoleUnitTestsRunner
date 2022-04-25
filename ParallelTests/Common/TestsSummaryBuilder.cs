using System.Globalization;
using System.Text;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelTests.Common
{
    public class TestsSummaryBuilder : ITestsSummaryBuilder
    {
        public string Build(ITestsMetricsCalculator metrics, double totalTimeInSeconds)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat(
                "Tests run: {0}, Errors: {1}, Failures: {2}, Inconclusive: {3}, Time: {4} seconds\n  Not run: {5}, Invalid: {6}, Ignored: {7}, Skipped: {8}",
                metrics.Passed,
                metrics.Errors,
                metrics.Failures,
                metrics.Inconclusive,
                string.Format(CultureInfo.InvariantCulture, "{0:0.###}", totalTimeInSeconds),
                metrics.NotRun.Length,
                metrics.Invalid,
                metrics.Ignored,
                metrics.Skipped
                );

            if (metrics.Failed.Any())
            {
                var reportIndex = 0;
                sb.AppendLine();
                sb.AppendLine("Errors and Failures:");
                WriteErrorsAndFailures(sb, metrics.All, ref reportIndex);
            }

            PrintArray(sb, "Tests Not Run:", metrics.NotRun);

            sb.AppendLine();
            return sb.ToString();
        }

        private static void WriteErrorsAndFailures(StringBuilder sb, IEnumerable<ResultMessage> results, ref int reportIndex)
        {
            foreach (var result in results.Where(x => x.Executed()))
            {
                if (result.HasResults())
                {
                    if (!result.IsFailed()) continue;
                    if (result.FailureSite == ResultMessage.Types.TestFailedOn.SetUp ||
                        result.FailureSite == ResultMessage.Types.TestFailedOn.TearDown)
                    {
                        WriteSingleResult(sb, result, ref reportIndex);
                    }
                }
                else if (result.IsFailed())
                {
                    WriteSingleResult(sb, result, ref reportIndex);
                }
            }
        }

        private static void WriteSingleResult(StringBuilder sb, ResultMessage result, ref int reportIndex)
        {
            var status = result.IsFailed()
                ? $"{result.FailureSite} {result.State}"
                : result.State.ToString();

            sb.AppendLine();
            sb.AppendFormat("{0}) {1} : {2}", ++reportIndex, status, result.FullName);

            if (!string.IsNullOrEmpty(result.Message))
            {
                sb.AppendLine();
                sb.AppendFormat("   {0}", result.Message);
            }

            if (!string.IsNullOrEmpty(result.StackTrace))
            {
                sb.AppendLine(result.IsFailure()
                    ? result.StackTrace
                    : result.StackTrace + Environment.NewLine);
            }
        }

        private static void PrintArray(StringBuilder sb, string message, ResultMessage[] items)
        {
            if (items.Length == 0) return;
            sb.AppendLine();
            sb.AppendLine(message);
            var i = 0;
            foreach (var r in items)
            {
                sb.AppendLine();
                sb.AppendFormat("{0}) {1} : {2}", ++i, r.State,
                    r.FullName + (string.IsNullOrEmpty(r.Message) ? string.Empty : ("\n   " + r.Message))
                );
            }
            sb.AppendLine();
        }
    }
}
