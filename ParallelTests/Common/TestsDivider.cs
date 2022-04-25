using Mnk.Library.Common.Tools;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelTests.Common
{
    public class TestsDivider : ITestsDivider
    {
        private readonly IOrderOptimizationManager orderOptimizationManager;

        public TestsDivider(IOrderOptimizationManager orderOptimizationManager)
        {
            this.orderOptimizationManager = orderOptimizationManager;
        }

        public IList<IList<ResultMessage>> Divide(TestsConfig config, ITestsMetricsCalculator metrics, IList<ResultMessage> checkedTests)
        {
            ResetTests(metrics, checkedTests);
            var filter = GetFilter(config.Categories, config.IncludeCategories);
            var items = checkedTests ?? metrics.Tests;
            return (config.ProcessCount > 1)
                                ? DivideTestsToRun(config, items.Where(filter).ToArray()).ToArray()
                                : new IList<ResultMessage>[] { items.Where(filter).ToArray() };
        }

        private static void ResetTests(ITestsMetricsCalculator metrics, IList<ResultMessage> checkedTests)
        {
            var items = checkedTests ?? metrics.All;
            foreach (var i in items)
            {
                i.State = ResultMessage.Types.TestResultState.NotRunnable;
                i.FailureSite = ResultMessage.Types.TestFailedOn.Test;
                i.Message = i.StackTrace = i.Description = string.Empty;
                i.Duration = i.AssertCount = 0;
                i.Output = string.Empty;
            }
        }

        private IEnumerable<IList<ResultMessage>> DivideTestsToRun(TestsConfig config, IList<ResultMessage> tests)
        {
            if (config.OptimizeOrder && config.ProcessCount > 1)
            {
                tests = orderOptimizationManager.Optimize(config.TestDllPath, tests);
            }
            var result = new List<IList<ResultMessage>>();
            for (var j = 0; j < config.ProcessCount; ++j)
            {
                result.Add(new List<ResultMessage>(tests.Count / config.ProcessCount));
            }
            for (var i = 0; i < tests.Count;)
            {
                for (var j = 0; j < config.ProcessCount && i < tests.Count; ++j)
                {
                    result[j].Add(tests[i++]);
                }
            }
            return result;
        }

        private static bool IncludeFilter(ResultMessage r, IEnumerable<string> values)
        {
            return r.Categories.Any(o => values.Any(x => x.EqualsIgnoreCase(o)));
        }

        private static bool ExcludeFilter(ResultMessage r, IEnumerable<string> values)
        {
            return r.Categories.All(o => !values.Any(x => x.EqualsIgnoreCase(o)));
        }

        private static Func<ResultMessage, bool> GetFilter(string[] categories, bool? include)
        {
            if (categories == null || !include.HasValue || categories.Length <= 0) return _ => true;
            if (include.Value)
            {
                return r => IncludeFilter(r, categories);
            }
            return r => ExcludeFilter(r, categories);
        }
    }
}
