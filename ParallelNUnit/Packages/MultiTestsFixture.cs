using LightInject;
using Mnk.Library.Common.Log;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Packages
{
    public sealed class MultiTestsFixture : IMultiTestsFixture
    {
        private readonly ILog log = LogManager.GetLogger<MultiTestsFixture>();

        public IList<TestsExecutionContext> Refresh(IEnumerable<TestsConfig> configs, int assembliesInParallel)
        {
            return ((assembliesInParallel > 1) ?
                configs.AsParallel().Select(Collect) :
                configs.Select(Collect)
                ).ToArray();
        }

        public void Run(IEnumerable<TestsConfig> configs, int assembliesInParallel, IList<TestsExecutionContext> executionContexts, ITestsUpdater updater, Action<TestsExecutionContext> onEnd = null, IList<ResultMessage> checkedTests = null)
        {
            if (assembliesInParallel > 1)
            {
                Parallel.ForEach(executionContexts,
                    new ParallelOptions { MaxDegreeOfParallelism = assembliesInParallel },
                    assembly => RunTest(assembly, updater, onEnd, checkedTests)
                    );
            }
            else
            {
                foreach (var context in executionContexts)
                {
                    RunTest(context, updater, onEnd, checkedTests);
                }
            }
        }

        private static void RunTest(TestsExecutionContext context, ITestsUpdater testsUpdater, Action<TestsExecutionContext> onEnd, IList<ResultMessage> checkedTests)
        {
            context.StartTime = Environment.TickCount;
            if (checkedTests != null)
            {
                var all = context.Results.Metrics.Tests;
                checkedTests = checkedTests
                    .Where(x => all.Any(a => string.Equals(x.Key, a.Key)))
                    .ToArray();
            }
            if (checkedTests == null || checkedTests.Any())
            {
                context.Results = context.TestsFixture.Run(context.Config, context.Results, testsUpdater, checkedTests);
            }
            if (context.Results.IsFailed || context.Results.Metrics.FailedCount > 0)
            {
                context.RetValue = -2;
            }
            onEnd?.Invoke(context);
        }

        private TestsExecutionContext Collect(TestsConfig config)
        {
            var context = new TestsExecutionContext
            {
                Path = config.TestDllPath,
                Config = config,
                RetValue = 0,
                Container = ServicesRegistrar.Register(),
            };
            context.TestsFixture = context.Container.GetInstance<ITestsFixture>();
            if (!context.TestsFixture.EnsurePathIsValid(context.Config))
            {
                log.Write("Incorrect path: " + config.TestDllPath);
                context.RetValue = -3;
            }
            else
            {
                context.Results = context.TestsFixture.Refresh(context.Config);
                if (!context.Results.IsFailed) return context;
                log.Write("Can't calculate tests count");
                context.RetValue = -3;
            }
            return context;
        }
    }
}
