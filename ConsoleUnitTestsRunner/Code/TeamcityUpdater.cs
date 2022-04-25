using Mnk.Library.Common.MT;
using Mnk.ParallelTests.Common;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    class TeamcityUpdater : GroupUpdater
    {
        private readonly object sync = new();
        public TeamcityUpdater(IUpdater updater, int totalCount)
            : base(updater, totalCount)
        {
        }

        public override void Update(string text)
        {
        }

        protected override void ProcessResults(int allCount, ResultMessage[] items, ISynchronizer synchronizer, TestsConfig config)
        {
            lock (sync)
            {
                foreach (var result in items.Where(x => x.IsTest()))
                {
                    var name = Escape(Path.GetFileName(config.TestDllPath) + ": " + result.FullName);
                    Console.WriteLine("##teamcity[testStarted name='{0}']", name);
                    if (!string.IsNullOrEmpty(result.Output))
                    {
                        Console.WriteLine("##teamcity[testStdOut name='{0}' out='{1}']", name, Escape(result.Output));
                    }
                    if (result.IsIgnored() || result.IsInconclusive() || result.IsSkipped())
                    {
                        Console.WriteLine("##teamcity[testIgnored name='{0}' message='{1}']", name, Escape(result.Message));
                    }
                    if (result.IsFailed())
                    {
                        Console.WriteLine("##teamcity[testFailed name='{0}' message='{1}' details='{2}']", name, Escape(result.Message), Escape(result.StackTrace));
                    }
                    Console.WriteLine("##teamcity[testFinished name='{0}' duration='{1}']", name, result.Duration * 1000);
                }
            }
        }

        private static string Escape(string text)
        {
            return text != null ? text
                .Replace("|", "||")
                .Replace("'", "|'")
                .Replace("\n", "|n")
                .Replace("\r", "|r")
                .Replace("\u0085", "|x")
                .Replace("\u2028", "|l")
                .Replace("\u2029", "|p")
                .Replace("[", "|[")
                .Replace("]", "|]")
               : null;
        }
    }
}
