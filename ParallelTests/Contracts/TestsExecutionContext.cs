using LightInject;

namespace Mnk.ParallelTests.Contracts
{
    public sealed class TestsExecutionContext : IDisposable
    {
        public string Path { get; set; }
        public int RetValue { get; set; }
        public TestsConfig Config { get; set; }
        public TestsResults Results { get; set; }
        public ITestsFixture TestsFixture { get; set; }
        public IServiceContainer Container { get; set; }
        public int StartTime { get; set; }

        public TestsExecutionContext()
        {
            RetValue = 0;
        }

        public void Dispose()
        {
            if (Container == null) return;
            Container.Dispose();
            Container = null;
        }
    }
}
