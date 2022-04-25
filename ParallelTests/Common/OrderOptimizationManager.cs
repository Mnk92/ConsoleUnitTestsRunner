using System.Text.Json;
using Mnk.Library.Common.Log;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelTests.Common
{
    public class OrderOptimizationManager : IOrderOptimizationManager
    {
        private static readonly string PrefetchFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TBox", "Tools", "NUnitRunner");
        private readonly ILog log = LogManager.GetLogger<OrderOptimizationManager>();

        public IList<ResultMessage> Optimize(string path, IList<ResultMessage> tests)
        {
            try
            {
                var cache = GetFilePath(path);
                if (!File.Exists(cache)) return tests;
                var str = File.ReadAllText(cache);
                var optimized = str.Split(',').ToList();
                return tests.OrderBy(x =>
                {
                    var id = optimized.IndexOf(x.Id.ToString());
                    return id < 0 ? int.MaxValue : id;
                }).ToList();
            }
            catch (Exception ex)
            {
                log.Write(ex, "Unexpected error");
            }
            return tests;
        }

        public void SaveStatistic(string path, IList<ResultMessage> tests)
        {
            try
            {
                var cache = GetFilePath(path);
                if (File.Exists(cache)) File.Delete(cache);
                using var f = File.OpenWrite(cache);
                JsonSerializer.Serialize(f, tests.OrderBy(x => x.Duration).Select(x => x.Id).ToArray());
            }
            catch (Exception ex)
            {
                log.Write(ex, "Unexpected error");
            }
        }

        private static string GetFilePath(string path)
        {
            if (!Directory.Exists(PrefetchFolder)) Directory.CreateDirectory(PrefetchFolder);
            return Path.Combine(PrefetchFolder, Path.GetFileNameWithoutExtension(path) + ".config");
        }
    }
}
