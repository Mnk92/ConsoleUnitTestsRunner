﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Tests.ParallelNUnit
{
    [TestClass]
    [TestCategory("Integration")]
    public class ThreadNUnitTestFixture : TestFixture
    {
        [TestInitialize]
        public override void SetUp()
        {
            Config = new TestsConfig
            {
                DirToCloneTests = Path.GetTempPath(),
                Mode = TestsRunnerMode.Internal
            };
            base.SetUp();
        }
    }
}
