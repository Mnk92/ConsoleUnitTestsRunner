using System.Globalization;
using System.Text;
using Mnk.Library.Interprocess;
using Mnk.ParallelTests.Contracts;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Mnk.ParallelNUnit.Core
{
    [Serializable]
    public sealed class RemoteListener : ITestListener
    {
        private StringBuilder Output { get; set; }
        public static bool ShouldStop = false;
        public string Handle { private get; set; }
        public bool Fast { private get; set; }
        public bool NeedOutput { private get; set; }

        private int time = Environment.TickCount;
        private InterprocessClient<ITestsClient.ITestsClientClient> client;
        private readonly List<ResultMessage> items = new();
        private int expectedTestCount;

        public RemoteListener()
        {
            Output = new StringBuilder();
        }

        private InterprocessClient<ITestsClient.ITestsClientClient> GetClient()
        {
            return client ??= new InterprocessClient<ITestsClient.ITestsClientClient>(Handle);
        }

        public void RunStarted(string name, int testCount)
        {
            expectedTestCount = testCount - 1;
        }

        public void RunFinished(TestResult result)
        {
            SendAll();
            client.Dispose();
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestStarted(ITest test)
        {
        }

        public void TestFinished(ITestResult result)
        {
            SaveResult(result);
            if (!Fast)
            {
                if (--expectedTestCount == 0)
                {
                    SendAll();
                    WaitUntilAllOtherTestsFinished();
                    return;
                }
            }
            if (Environment.TickCount - time <= 3000) return;
            SendAll();
            time = Environment.TickCount;
        }

        private void SaveResult(ITestResult result)
        {
            items.Add(new ResultMessage
            {
                Id = int.Parse(result.Test.Id, CultureInfo.InvariantCulture),
                Key = result.Test.Name,
                FullName = result.Test.FullName,
                Description = result.Test.Properties.Get(PropertyNames.Description)?.ToString(),
                Message = result.Message,
                StackTrace = result.StackTrace,
                State = MapResultState(result.ResultState),
                FailureSite = MapFailureSite(result.ResultState.Site),
                Duration = result.Duration,
                Type = result.Test.TestType,
                AssertCount = result.AssertCount,
                Output = Output.ToString(),
            });
            if (NeedOutput) Output.Clear();
        }

        public void SuiteStarted(ITest test)
        {
        }

        public void SuiteFinished(TestResult result)
        {
            SaveResult(result);
        }

        public void UnhandledException(Exception exception)
        {
            Console.WriteLine("Unhandled exception:");
            Console.WriteLine(exception.ToString());
        }

        public void TestOutput(TestOutput testOutput)
        {
            if (NeedOutput) Output.AppendLine(testOutput.Text);
        }

        public void SendMessage(TestMessage message)
        {
            if (NeedOutput) Output.AppendLine(message.Message);
        }

        private void WaitUntilAllOtherTestsFinished()
        {
            var s = new TestsServer();
            using var server = new InterprocessServer<ITestsServer.ITestsServerBase, ITestsClient.ITestsClientClient>(s);
            GetClient().Instance.CanFinish(new StringMessage { Value = server.Handle });
            while (s.ShouldWait)
            {
                Thread.Sleep(20);
            }
        }
        private void SendAll()
        {
            if (items.Count == 0) return;
            ShouldStop = GetClient().Instance.SendTestsResults(new ResultsMessage { Items = { items } }).Value;
            items.Clear();
        }
        private ResultMessage.Types.TestFailedOn MapFailureSite(FailureSite site)
        {
            return site switch
            {
                FailureSite.Child => ResultMessage.Types.TestFailedOn.Child,
                FailureSite.Parent => ResultMessage.Types.TestFailedOn.Parent,
                FailureSite.SetUp => ResultMessage.Types.TestFailedOn.SetUp,
                FailureSite.TearDown => ResultMessage.Types.TestFailedOn.TearDown,
                FailureSite.Test => ResultMessage.Types.TestFailedOn.Test,
                _ => throw new ArgumentException($"Unknown failure site: {site}.")
            };
        }

        private ResultMessage.Types.TestResultState MapResultState(ResultState state)
        {
            if (state == ResultState.Cancelled)
            {
                return ResultMessage.Types.TestResultState.Cancelled;
            }
            if (state == ResultState.ChildFailure)
            {
                return ResultMessage.Types.TestResultState.ChildFailure;
            }
            if (state == ResultState.ChildIgnored)
            {
                return ResultMessage.Types.TestResultState.ChildIgnored;
            }
            if (state == ResultState.ChildWarning)
            {
                return ResultMessage.Types.TestResultState.ChildWarning;
            }
            if (state == ResultState.Error)
            {
                return ResultMessage.Types.TestResultState.Error;
            }
            if (state == ResultState.Explicit)
            {
                return ResultMessage.Types.TestResultState.Explicit;
            }
            if (state == ResultState.Failure)
            {
                return ResultMessage.Types.TestResultState.Failure;
            }
            if (state == ResultState.Ignored)
            {
                return ResultMessage.Types.TestResultState.Ignored;
            }
            if (state == ResultState.Inconclusive)
            {
                return ResultMessage.Types.TestResultState.Inconclusive;
            }
            if (state == ResultState.NotRunnable)
            {
                return ResultMessage.Types.TestResultState.NotRunnable;
            }
            if (state == ResultState.SetUpError)
            {
                return ResultMessage.Types.TestResultState.SetUpError;
            }
            if (state == ResultState.SetUpFailure)
            {
                return ResultMessage.Types.TestResultState.SetUpFailure;
            }
            if (state == ResultState.Skipped)
            {
                return ResultMessage.Types.TestResultState.Skipped;
            }
            if (state == ResultState.Success)
            {
                return ResultMessage.Types.TestResultState.Success;
            }
            if (state == ResultState.TearDownError)
            {
                return ResultMessage.Types.TestResultState.TearDownError;
            }
            if (state == ResultState.Warning)
            {
                return ResultMessage.Types.TestResultState.Warning;
            }
            throw new ArgumentException($"Unknown result state: {state}.");
        }
    }
}
