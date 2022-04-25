using System.Text;
using Mnk.Library.Common.MT;
using Mnk.ParallelTests.Common;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    class NUnitLabelsUpdater : GroupUpdater
    {
        public NUnitLabelsUpdater(IUpdater updater, int totalCount) : base(updater, totalCount) { }
        public override void Update(string text)
        {
        }

        protected override void ProcessResults(int allCount, ResultMessage[] items, ISynchronizer synchronizer, TestsConfig config)
        {
            if (items == null) return;

            var sb = new StringBuilder();
            foreach (var result in items)
            {
                sb.AppendFormat("***** {0}", result.FullName).AppendLine();
            }
            Console.Write(sb);
        }
    }
}
