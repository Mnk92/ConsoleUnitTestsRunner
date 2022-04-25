using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Core
{
    class TestsServer : ITestsServer.ITestsServerBase
    {
        public bool ShouldWait { get; private set; }
        public TestsServer()
        {
            ShouldWait = true;
        }
        public override Task<Empty> CanClose(Empty request, ServerCallContext context)
        {
            ShouldWait = false;
            return Task.FromResult(new Empty());
        }
    }
}
