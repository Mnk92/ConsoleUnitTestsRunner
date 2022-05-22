using NUnit.Framework;

namespace NUnitTests.Example
{
    public class TestsFixture
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Setup");
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown");
        }

        [Test]
        public void SuccessTest()
        {
            Assert.Pass();
        }

        [Test]
        [Pairwise]
        public void SuccessPairwiseTest([Values(1, 2, 3, 4, 5)] int x, [Values(1, 2, 3, 4, 5)] int y)
        {
            Assert.Pass();
        }

        [Test]
        [Ignore("example")]
        public void IgnoredTest()
        {
            Assert.Pass();
        }

        [Test]
        [Explicit]
        public void ExplicitTest()
        {
            Assert.Pass();
        }
    }
}