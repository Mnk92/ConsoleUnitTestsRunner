using Mnk.Library.Common.Log;
using Mnk.ParallelNUnit.Core;
using Mnk.ConsoleUnitTestsRunner.Code.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    class Executor : IExecutor
    {
        private readonly ILog log = LogManager.GetLogger<Executor>();
        private readonly IInfoView view;
        private readonly IConsoleTestsExecutor executor;

        public Executor(IInfoView view, IConsoleTestsExecutor executor)
        {
            this.view = view;
            this.executor = executor;
        }

        public int Execute(string[] args)
        {
            if (args.Length <= 0 || string.Equals(args[0], "/?") || string.Equals(args[0], "/help", StringComparison.OrdinalIgnoreCase))
            {
                view.ShowHelp();
                return -1;
            }

            var ret = -1;
            var wait = false;
            try
            {
                var cmdArgs = new CommandLineArgs(args);
                wait = cmdArgs.Wait;
                if (cmdArgs.Logo)
                {
                    view.ShowLogo();
                    view.ShowArgs(cmdArgs);
                }
                if (!cmdArgs.Paths.Any())
                {
                    view.ShowHelp();
                    return -1;
                }
                var notExist = cmdArgs.Paths.Where(x => !File.Exists(x)).ToArray();
                if (notExist.Any())
                {
                    log.Write("Can't find files: " + string.Join(" ", notExist));
                    return -2;
                }

                Console.WriteLine("ProcessModel: Default\tDomainUseage: Single");
                Console.WriteLine("Execution Runtime: Default\tCPUCount: " + Environment.ProcessorCount);

                ret = executor.Run(cmdArgs);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Internal error");
                view.ShowHelp();
            }

            if (wait) Console.ReadKey();
            return ret;
        }
    }
}
