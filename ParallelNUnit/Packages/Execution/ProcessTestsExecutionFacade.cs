using System.Diagnostics;
using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Mnk.Library.Interprocess;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Packages.Execution
{
    class ProcessTestsExecutionFacade : ITestsExecutionFacade
    {
        public TestsResults CollectTests(TestsConfig config, InterprocessClient<ITestsClient.ITestsClientClient> server)
        {
            Execute(() => Create(config, server.Handle, TestsCommands.Collect));
            return new TestsResults(server.Instance.GetCollected(new Empty()).Items);
        }

        public virtual void Run(TestsConfig config, string handle)
        {
            Execute(() => Create(config, handle, config.NeedSynchronizationForTests ? TestsCommands.Test : TestsCommands.FastTest));
        }

        protected static void Execute(Func<Process> operation)
        {
            Process context = null;
            try
            {
                context = operation();
            }
            finally
            {
                if (context != null)
                {
                    context.WaitForExit();
                    context.Dispose();
                }
            }
        }

        protected static Process Create(TestsConfig config, string handle, string command)
        {
            var fileName = config.NUnitAgentPath;
            var args = string.Format(CultureInfo.InvariantCulture, "{0} \"{1}\" {2} {3}", handle, config.TestDllPath, command, config.RuntimeFramework ?? string.Empty);
            if (config.RunAsx86) ApplyCommand(Path.Combine(Environment.CurrentDirectory, config.RunAsx86Path), ref args, ref fileName);
            if (!File.Exists(config.NUnitAgentPath)) throw new ArgumentException("Can't find NUnit agent: " + config.NUnitAgentPath);
            var pi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = config.RunAsAdmin,
            };
            if (config.RunAsAdmin && !string.Equals(command, TestsCommands.Collect, StringComparison.OrdinalIgnoreCase)) pi.Verb = "runas";
            return Process.Start(pi);
        }

        private static void ApplyCommand(string command, ref string args, ref string fileName)
        {
            args = string.Format(CultureInfo.InvariantCulture, "\"{0}\" {1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), args);
            fileName = command;
        }

    }
}
