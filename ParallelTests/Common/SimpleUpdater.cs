﻿using System.Globalization;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelTests.Common
{
    public class SimpleUpdater : ITestsUpdater
    {
        private readonly IUpdater updater;
        private int passedCount = 0;
        private int failedCount = 0;
        private readonly object locker = new();

        public SimpleUpdater(IUpdater updater)
        {
            this.updater = updater;
        }

        public virtual void Update(string text)
        {
            updater.Update(i => string.Format(CultureInfo.InvariantCulture, "{0}, time: {1}", text, i.FormatTimeInSec()), 0, 1);
        }

        public bool UserPressClose => updater.UserPressClose;

        public void Update(int allCount, ResultMessage[] items, int failed, ISynchronizer synchronizer, TestsConfig config)
        {
            lock (locker)
            {
                passedCount += items.Length;
                failedCount += failed;
            }
            ProcessResults(allCount, items, synchronizer, config);
        }

        protected virtual void ProcessResults(int allCount, ResultMessage[] items, ISynchronizer synchronizer, TestsConfig config)
        {
            var caption = string.Format(CultureInfo.InvariantCulture, "Tested: {0}/{1}, failed: {2}, finished = {3}/{4}",
                                        passedCount,
                                        allCount, failedCount,
                                        synchronizer.Finished, config.ProcessCount);
            updater.Update(i => string.Format(CultureInfo.InvariantCulture, "{0}, time: {1}", caption, i.FormatTimeInSec()), passedCount, allCount);
        }
    }
}
