namespace Mnk.ParallelTests.Contracts
{
    public class TestsConfig
    {
        public string TestDllPath { get; set; }
        public string DirToCloneTests { get; set; }
        public string CommandBeforeTestsRun { get; set; }
        public string RuntimeFramework { get; set; }
        public bool NoThreading { get; set; }
        public int Timeout { get; set; }
        public bool CopyToSeparateFolders { get; set; }
        public string[] CopyMasks { get; set; }
        public bool NeedSynchronizationForTests { get; set; }
        public int StartDelay { get; set; }
        public bool NeedOutput { get; set; }
        public bool OptimizeOrder { get; set; }
        public int ProcessCount { get; set; }
        public string[] Categories { get; set; }
        public bool? IncludeCategories { get; set; }
        public string Mode { get; set; }
        public string NUnitAgentPath { get; set; }
        public string RunAsx86Path { get; set; }
        public bool ShadowCopyFiles { get; set; }
        public bool RunAsx86 { get; set; }
        public bool RunAsAdmin { get; set; }
        public bool SkipChildrenOnCalculateTests { get; set; }
        public TestsConfig()
        {
            SkipChildrenOnCalculateTests = false;
            ShadowCopyFiles = false;
        }

    }
}
