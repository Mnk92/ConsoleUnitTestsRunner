﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Tests.ParallelNUnit
{
    [TestClass]
    [TestCategory("Integration")]
    public class SingleProcessNUnit64TestFixture : TestFixture
    {
        private const string NUnitAgentPath = "NUnitAgent.exe";
        private const string RunAsx86Path = "RunAsx86.exe";

        [TestInitialize]
        public override void SetUp()
        {
            Config = new TestsConfig
            {
                NUnitAgentPath = NUnitAgentPath,
                RunAsx86Path = RunAsx86Path,
                DirToCloneTests = Path.GetTempPath(),
                RunAsAdmin = false,
                RunAsx86 = false,
                Mode = TestsRunnerMode.Process
            };
            base.SetUp();
        }
    }
}
