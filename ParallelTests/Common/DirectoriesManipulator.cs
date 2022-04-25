using System.Globalization;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ParallelTests.Common
{
    public class DirectoriesManipulator : IDirectoriesManipulator
    {
        private readonly ICopyDirGenerator copyDirGenerator;
        private static readonly ILog Log = LogManager.GetLogger<DirectoriesManipulator>();

        public DirectoriesManipulator(ICopyDirGenerator copyDirGenerator)
        {
            this.copyDirGenerator = copyDirGenerator;
        }

        public IList<string> GenerateFolders(TestsConfig config, ITestsUpdater updater, int count)
        {
            var dllPaths = new List<string>();
            if (config.CopyToSeparateFolders)
            {
                updater.Update("Start cloning of the unit tests folder");
                CopyToLocalFolders(config, updater, count, dllPaths);
                updater.Update("Cloning of the unit tests folder finished");
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    dllPaths.Add(config.TestDllPath);
                }
            }
            return dllPaths;
        }

        private void CopyToLocalFolders(TestsConfig config, ITestsUpdater updater, int count, List<string> dllPaths)
        {
            var files = copyDirGenerator.GetFiles(config.TestDllPath, config.CopyMasks, out var name, out var sourceDir);
            foreach (var folder in GenerateFolders(count, config.DirToCloneTests))
            {
                updater.Update("Copy files to: " + folder);
                foreach (var item in files)
                {
                    if (updater.UserPressClose) break;
                    var target = Path.Combine(folder, item.Key);
                    if (!Directory.Exists(target)) Directory.CreateDirectory(target);
                    foreach (var file in item.Value)
                    {
                        File.Copy(Path.Combine(sourceDir, item.Key, file), Path.Combine(target, file));
                    }
                }
                if (updater.UserPressClose) break;
                dllPaths.Add(Path.Combine(folder, name));
            }
        }

        private static IEnumerable<string> GenerateFolders(int count, string dirToCloneTests)
        {
            var folders = new List<string>(count);
            var fileId = 0;
            for (var i = 0; i < count; ++i)
            {
                string folder;
                while (true)
                {
                    folder = Path.Combine(dirToCloneTests, (++fileId).ToString(CultureInfo.InvariantCulture));
                    if (!File.Exists(folder) && !Directory.Exists(folder)) break;
                }
                folders.Add(folder);
            }
            return folders;
        }

        public void ClearFolders(TestsConfig config, IList<string> dllPaths)
        {
            if (!config.CopyToSeparateFolders) return;
            var copyDeep = copyDirGenerator.CalcCopyDeep(copyDirGenerator.NormalizePaths(config.CopyMasks));
            foreach (var t in dllPaths)
            {
                try
                {
                    var name = Path.GetDirectoryName(t);
                    for (var j = 1; j < copyDeep; ++j)
                    {
                        name = Path.GetDirectoryName(name);
                    }
                    name = name ?? string.Empty;
                    if (Directory.Exists(name))
                    {
                        Directory.Delete(name, true);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex, "Can't delete folder: " + t);
                }
            }
        }

    }
}
