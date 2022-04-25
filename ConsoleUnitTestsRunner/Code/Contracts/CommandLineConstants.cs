namespace Mnk.ConsoleUnitTestsRunner.Code.Contracts
{
    static class CommandLineConstants
    {
        public const string TestsInParallel = "-p=";
        public const string AssembliesInParallel = "-ap=";
        public const string StartDelay = "-startDelay=";
        public const string Clone = "-clone";
        public const string CopyMasks = "-copyMasks=";
        public const string DirToCloneTests = "-dirToCloneTests=";
        public const string CommandBeforeTestsRun = "-commandBeforeTestsRun=";
        public const string Sync = "-sync";
        public const string Prefetch = "-prefetch";
        public const string Teamcity = "-teamcity";
        public const string Mode = "-mode=";
        public const string ReturnSuccess = "-returnSuccess";

        public const string Include = "/include=";
        public const string Exclude = "/exclude=";
        public const string Output = "/output:";
        public const string Xml = "/xml=";
        public const string Framework = "/framework=";
        public const string Timeout = "/timeout=";
        public const string NoThreading = "/nothread";
        public const string NoLogo = "/nologo";
        public const string Labels = "/labels";
        public const string NoShadow = "/noshadow";
        public const string Wait = "/wait";
    }
}
