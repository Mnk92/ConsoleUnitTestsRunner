using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.ConsoleUnitTestsRunner.Code.Contracts;
using Mnk.ParallelTests.Common;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    internal class ConsoleTestsExecutor : IConsoleTestsExecutor
    {
        private readonly IConsoleView view;
        private readonly IUpdater updater;
        private readonly IMultiTestsFixture multiTestsFixture;

        public ConsoleTestsExecutor(IConsoleView view, IUpdater updater, IMultiTestsFixture multiTestsFixture)
        {
            this.view = view;
            this.updater = updater;
            this.multiTestsFixture = multiTestsFixture;
        }

        public int Run(CommandLineArgs args)
        {
            var workingDirectory = Environment.CurrentDirectory;
            var configs = args.Paths.Select(args.ToTestsConfig).ToArray();
            if (args.Logo) Console.WriteLine("Calculating tests.");
            var contexts = multiTestsFixture.Refresh(configs, args.AssembliesInParallel);
            var retValue = -1;
            try
            {
                retValue = contexts.Min(x => x.RetValue);
                if (retValue == 0)
                {
                    var totalResults = new TestsResults(contexts.SelectMany(x => x.Results.Items).ToArray());
                    if (args.Logo)
                    {
                        Console.WriteLine("{0} tests found.", totalResults.Metrics.Total);
                        Console.WriteLine("Running tests.");
                    }

                    multiTestsFixture.Run(
                        configs,
                        args.AssembliesInParallel,
                        contexts,
                        BuildUpdater(args, updater, totalResults.Metrics.Total),
                        c => OnTestEnd(c, args));

                    view.GenerateTotalResults();
                    view.PrintTotalResults();
                    PrintTotalInfo(view, args.XmlReport, args.OutputReport, args.Paths.FirstOrDefault(),
                        workingDirectory);
                    return args.ReturnSuccess ? 0 : contexts.Min(x => x.RetValue);
                }
            }
            finally
            {
                foreach (var context in contexts)
                {
                    context.Dispose();
                }
            }
            return retValue;
        }

        private void OnTestEnd(TestsExecutionContext context, CommandLineArgs args)
        {
            view.AddResult(context.Results);
            if (args.Logo && !args.Labels && args.Paths.Count > 1)
            {
                Console.WriteLine("'{0}' is done, time: {1}", Path.GetFileName(context.Path), ((Environment.TickCount - context.StartTime) / 1000).FormatTimeInSec());
            }
        }

        private static ITestsUpdater BuildUpdater(CommandLineArgs args, IUpdater updater, int totalCount)
        {
            if (args.Teamcity) return new TeamcityUpdater(updater, totalCount);
            return args.Labels ?
                new NUnitLabelsUpdater(updater, totalCount) :
                new GroupUpdater(updater, totalCount);
        }

        private static void PrintTotalInfo(IConsoleView view, string xmlReport, string outputReport, string path, string dir)
        {
            Directory.SetCurrentDirectory(dir);
            if (!string.IsNullOrEmpty(xmlReport))
                view.GenerateXmlReport(path, xmlReport);

            if (!string.IsNullOrEmpty(outputReport))
            {
                var totalResult = view.TotalResult;
                File.WriteAllText(
                    Path.Combine(Environment.CurrentDirectory, outputReport),
                    string.Join(string.Empty, totalResult.Metrics.All.Select(x => x.Output)));
            }
        }
    }
}
