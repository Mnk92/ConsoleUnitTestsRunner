using Google.Protobuf.WellKnownTypes;
using Mnk.Library.Common.Models;
using Mnk.Library.Interprocess;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelNUnit.Packages.Common
{
    public class Synchronizer : ISynchronizer
    {
        private readonly IList<Pair<string, bool>> finishedAgents = new List<Pair<string, bool>>();

        public int Finished => finishedAgents.Count;

        public void ProcessNextAgent(TestsConfig config, string handle)
        {
            finishedAgents.Add(new Pair<string, bool>(handle, false));
            if (Finished != config.ProcessCount) return;
            foreach (var agent in finishedAgents)
            {
                if (agent.Value) continue;
                var id = agent.Key;
                agent.Value = true;
                ThreadPool.QueueUserWorkItem(
                    _ =>
                    {
                        using var cl = new InterprocessClient<ITestsServer.ITestsServerClient>(id);
                        cl.Instance.CanClose(new Empty());
                    });
            }
        }
    }
}
