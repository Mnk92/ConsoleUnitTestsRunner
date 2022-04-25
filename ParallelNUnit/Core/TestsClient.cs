using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Core
{
    class TestsClient : ITestsClient.ITestsClientBase
    {
        private readonly object sync = new();
        private ResultMessage[] Collection { get; set; }
        private readonly IDictionary<int, ResultMessage> allTestsResults;
        private int allTestsCount;
        private TestRunConfigMessage runConfig;
        private ISynchronizer synchronizer;
        private ITestsUpdater progress;
        private TestsConfig testsConfig;

        public TestsClient()
        {
            Collection = Array.Empty<ResultMessage>();
            allTestsResults = new Dictionary<int, ResultMessage>();
        }

        public override Task<Empty> SetCollectedTests(ResultsMessage request, ServerCallContext context)
        {
            Collection = request.Items.ToArray();
            return Task.FromResult(new Empty());
        }

        public override Task<TestRunConfigMessage> GiveMeConfig(Empty request, ServerCallContext context)
        {
            lock (sync)
            {
                if (runConfig == null)
                    throw new ArgumentNullException("Configuration can't be null. Did you forget to execute PrepareToRun?");
                if (string.Equals(testsConfig.Mode, TestsRunnerMode.MultiProcess))
                {
                    if (runConfig.DllPaths.Count > 1)
                    {
                        var result = new TestRunConfigMessage { DllPaths = { new[] { runConfig.DllPaths[0] } } };
                        result.TestsToRun.Add(runConfig.TestsToRun[0]);
                        runConfig.DllPaths.RemoveAt(0);
                        runConfig.TestsToRun.RemoveAt(0);
                        return Task.FromResult(result);
                    }
                }
                var message = runConfig;
                runConfig = null;
                return Task.FromResult(message);
            }
        }
        private void FillAllTests(IEnumerable<ResultMessage> allTests)
        {
            foreach (var item in allTests)
            {
                allTestsResults[item.Id] = item;
                FillAllTests(item.Children);
            }
        }
        private static void Map(ResultMessage item, ResultMessage i)
        {
            item.AssertCount = i.AssertCount;
            item.Description = i.Description;
            item.Message = i.Message;
            item.StackTrace = Filter(i.StackTrace);
            item.Output = i.Output;
            item.State = i.State;
            item.FailureSite = i.FailureSite;
        }
        public override Task<BoolMessage> SendTestsResults(ResultsMessage request, ServerCallContext context)
        {
            var failed = 0;
            lock (allTestsResults)
            {
                foreach (var i in request.Items)
                {
                    if (!allTestsResults.TryGetValue(i.Id, out var item)) continue;
                    item.Key = i.Key;
                    item.Duration = Math.Max(item.Duration, i.Duration);

                    if (i.IsTest())
                    {
                        Map(item, i);
                        if (i.IsFailed())
                        {
                            ++failed;
                        }
                    }
                    else
                    {
                        if (!item.IsFailed() && !(item.IsSuccess() && i.IsInconclusive()))
                        {
                            Map(item, i);
                        }
                    }
                }
            }
            progress.Update(allTestsCount, request.Items.Where(x => x.IsTest()).ToArray(), failed, synchronizer, testsConfig);
            return Task.FromResult(new BoolMessage { Value = progress.UserPressClose });
        }

        public override Task<Empty> CanFinish(StringMessage request, ServerCallContext context)
        {
            synchronizer.ProcessNextAgent(testsConfig, request.Value);
            return Task.FromResult(new Empty());
        }

        public void Prepare()
        {
            Collection = Array.Empty<ResultMessage>();
        }

        public override Task<ResultsMessage> GetCollected(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new ResultsMessage { Items = { Collection } });
        }

        public void PrepareToRun(ISynchronizer s, ITestsUpdater u, TestRunConfigMessage config, IList<IList<ResultMessage>> packages, IList<ResultMessage> allTests, ITestsMetricsCalculator metricsCalculator, TestsConfig testConfig)
        {
            synchronizer = s;
            testsConfig = testConfig;
            runConfig = config;
            allTestsCount = metricsCalculator.Total;
            allTestsResults.Clear();
            progress = u;
            foreach (var package in packages)
            {
                runConfig.TestsToRun.Add(new TestRunConfigMessage.Types.TestsIds { Ids = { package.Select(x => x.Id).ToArray() } });
            }
            FillAllTests(allTests);
        }


        private static string Filter(string stack)
        {
            if (stack == null) return null;
            using var sw = new StringWriter();
            using var sr = new StringReader(stack);
            try
            {
                while (sr.ReadLine() is { } line)
                {
                    if (!FilterLine(line))
                    {
                        sw.WriteLine(line.Trim());
                    }
                }
            }
            catch (Exception)
            {
                return stack;
            }
            return sw.ToString();
        }

        private static bool FilterLine(string line)
        {
            var patterns = new[]
            {
                "NUnit.Core.TestCase",
                "NUnit.Core.ExpectedExceptionTestCase",
                "NUnit.Core.TemplateTestCase",
                "NUnit.Core.TestResult",
                "NUnit.Core.TestSuite",
                "NUnit.Framework.Assertion",
                "NUnit.Framework.Assert",
                "System.Reflection.MonoMethod",
                "Mnk.Library.ParallelNUnit",
                "Mnk.Library.Common"
            };

            return patterns.Any(t => line.IndexOf(t, StringComparison.Ordinal) > 0);
        }
    }
}
