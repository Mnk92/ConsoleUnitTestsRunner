using System.Globalization;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code.Contracts
{
    class CommandLineArgs
    {
        public IList<string> Paths { get; private set; }
        public int TestsInParallel { get; private set; }
        public int AssembliesInParallel { get; private set; }
        public bool Prefetch { get; private set; }
        public bool Clone { get; private set; }
        public string[] CopyMasks { get; private set; }
        public bool Sync { get; private set; }
        public bool Teamcity { get; private set; }
        public string DirToCloneTests { get; private set; }
        public string CommandBeforeTestsRun { get; private set; }
        public int StartDelay { get; private set; }
        public string Mode { get; private set; }
        public bool ReturnSuccess { get; private set; }

        //NUnit
        public string[] Include { get; private set; }
        public string[] Exclude { get; private set; }
        public bool Logo { get; private set; }
        public bool Labels { get; private set; }
        public string XmlReport { get; private set; }
        public string OutputReport { get; private set; }
        public bool Wait { get; private set; }
        public string RuntimeFramework { get; private set; }
        public bool NoThreading { get; private set; }
        public int Timeout { get; private set; }

        public CommandLineArgs()
        {
            TestsInParallel = Environment.ProcessorCount;
            AssembliesInParallel = 1;
            Prefetch = false;
            Clone = false;
            CopyMasks = new[] { "*.dll", "*.config" };
            Sync = false;
            XmlReport = string.Empty;
            DirToCloneTests = Path.GetTempPath();
            CommandBeforeTestsRun = string.Empty;
            StartDelay = 0;
            Timeout = 120000;
            NoThreading = false;
            Logo = true;
            Labels = false;
            Wait = false;
            Teamcity = false;
            Paths = new List<string>();
            Mode = TestsRunnerMode.Internal;
            ReturnSuccess = false;
        }

        public CommandLineArgs(params string[] args) : this()
        {
            foreach (var s in args)
            {
                ParseArgument(s);
            }
        }

        private void ParseArgument(string arg)
        {
            if (Starts(arg, CommandLineConstants.TestsInParallel))
            {
                TestsInParallel = int.Parse(arg.Substring(CommandLineConstants.TestsInParallel.Length), CultureInfo.InvariantCulture);
                if (TestsInParallel <= 0) TestsInParallel = Environment.ProcessorCount;
            }
            else if (Starts(arg, CommandLineConstants.AssembliesInParallel))
            {
                AssembliesInParallel = int.Parse(arg.Substring(CommandLineConstants.AssembliesInParallel.Length), CultureInfo.InvariantCulture);
                if (AssembliesInParallel <= 0) AssembliesInParallel = Environment.ProcessorCount;
            }
            else if (Starts(arg, CommandLineConstants.StartDelay))
            {
                StartDelay = int.Parse(arg.Substring(CommandLineConstants.StartDelay.Length), CultureInfo.InvariantCulture);
            }
            else if (Equals(arg, CommandLineConstants.Clone))
            {
                Clone = true;
            }
            else if (Starts(arg, CommandLineConstants.CopyMasks))
            {
                CopyMasks = arg.Substring(CommandLineConstants.CopyMasks.Length).Split(';');
            }
            else if (Starts(arg, CommandLineConstants.DirToCloneTests))
            {
                DirToCloneTests = arg.Substring(CommandLineConstants.DirToCloneTests.Length);
            }
            else if (Starts(arg, CommandLineConstants.CommandBeforeTestsRun))
            {
                CommandBeforeTestsRun = arg.Substring(CommandLineConstants.CommandBeforeTestsRun.Length);
            }
            else if (Equals(arg, CommandLineConstants.Sync))
            {
                Sync = true;
            }
            else if (Equals(arg, CommandLineConstants.Prefetch))
            {
                Prefetch = true;
            }
            else if (Equals(arg, CommandLineConstants.Teamcity))
            {
                Teamcity = true;
            }
            else if (Starts(arg, CommandLineConstants.Mode))
            {
                Mode = arg.Substring(CommandLineConstants.Mode.Length);
            }
            else if (Equals(arg, CommandLineConstants.ReturnSuccess))
            {
                ReturnSuccess = true;
            }
            //NUnit arguments
            else if (Starts(arg, CommandLineConstants.Include))
            {
                if (Exclude != null) throw new ArgumentException("You can't specify include and exclude at the same time.");
                Include = arg.Substring(CommandLineConstants.Include.Length).Split(';');
            }
            else if (Starts(arg, CommandLineConstants.Exclude))
            {
                if (Include != null) throw new ArgumentException("You can't specify include and exclude at the same time.");
                Exclude = arg.Substring(CommandLineConstants.Exclude.Length).Split(';');
            }
            else if (Starts(arg, CommandLineConstants.Output))
            {
                OutputReport = arg.Substring(CommandLineConstants.Output.Length);
            }
            else if (Starts(arg, CommandLineConstants.Xml))
            {
                XmlReport = arg.Substring(CommandLineConstants.Xml.Length);
            }
            else if (Starts(arg, CommandLineConstants.Framework))
            {
                RuntimeFramework = arg.Substring(CommandLineConstants.Framework.Length);
            }
            else if (Starts(arg, CommandLineConstants.Timeout))
            {
                Timeout = int.Parse(arg.Substring(CommandLineConstants.Timeout.Length), CultureInfo.InvariantCulture);
            }
            else if (Equals(arg, CommandLineConstants.NoThreading))
            {
                NoThreading = false;
            }
            else if (Equals(arg, CommandLineConstants.NoLogo))
            {
                Logo = false;
            }
            else if (Equals(arg, CommandLineConstants.Labels))
            {
                Labels = true;
            }
            else if (Equals(arg, CommandLineConstants.NoShadow))
            {
            }
            else if (Equals(arg, CommandLineConstants.Wait))
            {
                Wait = true;
            }
            else
            {
                Paths.Add(arg);
            }
        }

        public TestsConfig ToTestsConfig(string path)
        {
            return new TestsConfig
            {
                RunAsAdmin = false,
                RunAsx86 = !Environment.Is64BitProcess,
                RunAsx86Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RunAsx86.exe"),
                NUnitAgentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NUnitAgent.exe"),
                CopyMasks = CopyMasks,
                CommandBeforeTestsRun = CommandBeforeTestsRun,
                CopyToSeparateFolders = Clone,
                DirToCloneTests = DirToCloneTests,
                NeedOutput = !string.IsNullOrEmpty(OutputReport),
                NeedSynchronizationForTests = Sync,
                ProcessCount = TestsInParallel,
                RuntimeFramework = RuntimeFramework,
                NoThreading = NoThreading,
                Timeout = Timeout,
                StartDelay = StartDelay,
                TestDllPath = path,
                OptimizeOrder = Prefetch,
                Categories = Include ?? Exclude,
                Mode = Mode,
                IncludeCategories = (Include != null || Exclude != null) ? Include != null : null
            };
        }


        private static bool Equals(string arg, string str)
        {
            return string.Equals(arg, str, StringComparison.OrdinalIgnoreCase);
        }

        private static bool Starts(string arg, string str)
        {
            return arg.StartsWith(str, StringComparison.OrdinalIgnoreCase);
        }
    }
}
