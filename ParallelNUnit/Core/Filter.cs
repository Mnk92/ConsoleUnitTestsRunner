using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Mnk.ParallelNUnit.Core
{
    [Serializable]
    public class Filter : TestFilter
    {
        public ISet<int> Items { get; set; }
        public override bool Match(ITest test)
        {
            if (RemoteListener.ShouldStop) return false;
            if (test.RunState == RunState.Explicit) return false;
            return !string.Equals(test.TestType, "TestMethod", StringComparison.OrdinalIgnoreCase) ||
                   Items.Contains(int.Parse(test.Id, CultureInfo.InvariantCulture));
        }

        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            return parentNode.AddElement("filter");
        }
    }
}
