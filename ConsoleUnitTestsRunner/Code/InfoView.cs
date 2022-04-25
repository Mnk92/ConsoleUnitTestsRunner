using System.Globalization;
using System.Reflection;
using Mnk.ConsoleUnitTestsRunner.Code.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    class InfoView : IInfoView
    {
        public void ShowArgs(CommandLineArgs cmd)
        {
            Console.WriteLine(
                "Start with arguments: " +
                string.Join(" ", new object[]
                {
                    string.Join(" ", cmd.Paths),
                    Format(CommandLineConstants.TestsInParallel, cmd.TestsInParallel),
                    Format(CommandLineConstants.AssembliesInParallel, cmd.AssembliesInParallel),
                    Format(CommandLineConstants.Sync, cmd.Sync),
                    Format(CommandLineConstants.DirToCloneTests, cmd.DirToCloneTests),
                    Format(CommandLineConstants.Clone, cmd.Clone),
                    Format(CommandLineConstants.CopyMasks, cmd.CopyMasks),
                    Format(CommandLineConstants.CommandBeforeTestsRun, cmd.CommandBeforeTestsRun),
                    Format(CommandLineConstants.StartDelay, cmd.StartDelay),
                    Format(CommandLineConstants.Prefetch, cmd.Prefetch),
                    Format(CommandLineConstants.Teamcity, cmd.Teamcity),
                    Format(CommandLineConstants.Mode, cmd.Mode),

                    Format(CommandLineConstants.NoLogo, !cmd.Logo),
                    Format(CommandLineConstants.Labels, cmd.Labels),
                    Format(CommandLineConstants.Include, cmd.Include),
                    Format(CommandLineConstants.Exclude, cmd.Exclude),
                    Format(CommandLineConstants.Xml, cmd.XmlReport),
                    Format(CommandLineConstants.Output, cmd.OutputReport),
                    Format(CommandLineConstants.Framework, cmd.RuntimeFramework),
                    Format(CommandLineConstants.Timeout, cmd.Timeout),
                    Format(CommandLineConstants.NoThreading, cmd.NoThreading),
                    Format(CommandLineConstants.Wait, cmd.Wait)
                }));
        }

        private static string Format(string name, bool value)
        {
            return value ? name : string.Empty;
        }

        private static string Format(string name, int value)
        {
            return name + value;
        }

        private static string Format(string name, string value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}'{1}'", name, value);
        }

        private static string Format(string prefix, string[] items)
        {
            if (items == null) return string.Empty;
            return prefix + string.Join(";", items);
        }

        public void ShowLogo()
        {
            Console.WriteLine("TBox.ConsoleUnitTestsRunner version " + Assembly.GetCallingAssembly().GetName().Version);
            Console.WriteLine("Source: http://cpntr.codeplex.com/");
            Console.WriteLine("Copyright (C) 2010-2022 Aliaksandr Hrynko (Mnk).");
            Console.WriteLine("All Rights Reserved");
            Console.WriteLine();
            Console.WriteLine("Runtime Environment - ");
            Console.WriteLine("   OS Version: " + Environment.OSVersion);
            Console.WriteLine("  CLR Version: " + Environment.Version);
            Console.WriteLine();
        }

        public void ShowHelp()
        {
            Console.WriteLine("You should specify at least 1 parameter: path to unit tests (multiple separated by space).");
            Console.WriteLine("Other parameters:");
            Console.WriteLine("-p=N             - specify tests in parallel count, by default N = cpu cores count");
            Console.WriteLine("-ap=N            - specify assemblies in parallel count, by default N = 1, if N=0, ap will be = cpu cores count");
            Console.WriteLine("-clone           - clone unit tests folder, by default false");
            Console.WriteLine("-copyMasks=a[;b] - file masks to copy, by default =*.dll;*.config");
            Console.WriteLine("-sync            - enable sync for unit tests, by default false");
            Console.WriteLine("-dirToCloneTests=path - folder to clone tests, by default %temp%");
            Console.WriteLine("-commandBeforeTestsRun=exePath - command to run before execute tests, but after it clone , by default empty");
            Console.WriteLine("-startDelay=N    - delay before test starts, by default 0");
            Console.WriteLine("-prefetch        - use test run time statistic to optimize tests separation, by default false");
            Console.WriteLine("-teamcity        - teamcity inteagration: will show test progress, by default false");
            Console.WriteLine("-mode=mode       - Execution mode: Internal (no thread), Process (separate process to run tests for each assembly), MultiProcess (separate process to run tests for each assembly and to each tests parallel group), by default Internal");
            Console.WriteLine("-returnSuccess   - Return success exit code if tests failed, by default false");
            Console.WriteLine("NUnit compatible arguments:");
            Console.WriteLine("/include=a1[;a2] - include only tests with specified categories");
            Console.WriteLine("/exclude=a1[;a2] - exclude all tests with specified categories");
            Console.WriteLine("/xml=[path]        path for xml report, by default empty");
            Console.WriteLine("/framework=[v]     framework version to use, by default empty");
            Console.WriteLine("/timeout=[v]       Set timeout for each test case in milliseconds, by default 120000");
            Console.WriteLine("/output:[path]     path for tests output, by default empty");
            Console.WriteLine("/labels            Label each test in stdOut, by default false");
            Console.WriteLine("/noshadow          Do nothing (for now)");
            Console.WriteLine("/nologo            Do not display the logo, by default false");
            Console.WriteLine("/nothread          Disable use of a separate thread for tests, by default false");
            Console.WriteLine("/help              Display this page");
            Console.WriteLine("/wait              wait for input key after at the end, by default false");
        }

    }
}
