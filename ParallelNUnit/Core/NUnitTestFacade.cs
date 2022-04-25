using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Mnk.Library.Common.Log;
using Mnk.Library.Interprocess;
using Mnk.ParallelTests.Contracts;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Mnk.ParallelNUnit.Core
{
    public sealed class NUnitTestFacade : ITestFacade
    {
        private static readonly object Sync = new();
        private readonly ILog log = LogManager.GetLogger<NUnitTestFacade>();

        public int RunTests(TestsConfig config, string handle)
        {
            using var cl = new InterprocessClient<ITestsClient.ITestsClientClient>(handle);
            var testRunConfig = cl.Instance.GiveMeConfig(new Empty());
            Parallel.For(0, testRunConfig.TestsToRun.Count, i =>
            {
                var path = testRunConfig.DllPaths[i];
                var items = testRunConfig.TestsToRun[i].Ids.ToArray();
                if (i > 0 && testRunConfig.StartDelay > 0)
                {
                    Thread.Sleep(i * testRunConfig.StartDelay);
                }
                Run(handle, path, items, config);
            });
            return 0;
        }

        public int Run(string handle, string path, int[] items, TestsConfig config)
        {
            var result = -1;
            Execute(path, config,
                runner =>
                {
                    runner.Run(new RemoteListener { Handle = handle, Fast = !config.NeedSynchronizationForTests, NeedOutput = config.NeedOutput },
                                new Filter { Items = new HashSet<int>(items) }
                        );
                    result = items.Length;
                });
            return result;
        }

        public ResultMessage CollectTests(string path, TestsConfig config)
        {
            ResultMessage result = null;
            Execute(path, config,
                runner => result = CollectResults(runner.ExploreTests(new Filter()), Array.Empty<string>()));
            return result;
        }

        private void Execute(string path, TestsConfig config, Action<ITestAssemblyRunner> action)
        {
            var nUnitBase = new NUnitBase();
            var p = nUnitBase.CreatePackage(path, config);
            lock (Sync)
            {
                var data = p.LoadTests();
                log.Write(data);
                if (data.Length != 0)
                {
                    log.Write("Can't load: " + path);
                    return;
                }
            }
            action(p.Runner);
        }

        private static ResultMessage CollectResults(ITest result, IEnumerable<string> ownerCategories)
        {
            var categories = ownerCategories.Concat(GetCategories(result)).Distinct().ToArray();
            var item = CreateResult(result, categories);
            foreach (var r in result.Tests)
            {
                item.Children.Add(CollectResults(r, categories));
            }
            return item;
        }

        private static ResultMessage CreateResult(ITest result, string[] categories)
        {
            return new ResultMessage
            {
                Id = int.Parse(result.Id, CultureInfo.InvariantCulture),
                Key = result.Name,
                FullName = result.FullName,
                Categories = { categories },
                Type = result.TestType,
            };
        }

        private static IEnumerable<string> GetCategories(ITest result)
        {
            return (!result.Properties.ContainsKey(PropertyNames.Category) ?
                Array.Empty<string>() : new[] { result.Properties.Get(PropertyNames.Category)?.ToString() });
        }
    }
}
