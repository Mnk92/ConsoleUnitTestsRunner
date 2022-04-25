using Mnk.ParallelTests.Contracts;
using NUnit.Framework.Api;

namespace Mnk.ParallelNUnit.Core
{
    public class NUnitBase
    {
        /*
        private readonly ITestEngine engine;
        private readonly IResultService resultService;
        private readonly ITestFilterService filterService;
        */
        public NUnitBase()
        {
            /*
            engine = TestEngineActivator.CreateInstance();
            resultService = engine.Services.GetService<IResultService>();
            filterService = engine.Services.GetService<ITestFilterService>();
            engine.WorkDirectory = AppDomain.CurrentDomain.BaseDirectory;
            */
        }

        public FrameworkController CreatePackage(string path, TestsConfig config)
        {
            var controller = new FrameworkController(path, "", new Dictionary<string, object>());
            return controller;
            /*
            var testPackage = new TestPackage(Path.GetFullPath(path));
            testPackage.AddSetting(EnginePackageSettings.RequestedRuntimeFramework,
                string.IsNullOrEmpty(config.RuntimeFramework) ?
                RuntimeFramework.CurrentFramework :
                RuntimeFramework.Parse(config.RuntimeFramework));
            testPackage.AddSetting(EnginePackageSettings.RunAsX86, config.RunAsx86);
            testPackage.AddSetting(EnginePackageSettings.DisposeRunners, true);
            testPackage.AddSetting(EnginePackageSettings.ShadowCopyFiles, config.ShadowCopyFiles);
            testPackage.AddSetting(EnginePackageSettings.WorkDirectory, AppDomain.CurrentDomain.BaseDirectory);
            testPackage.AddSetting(EnginePackageSettings.SkipNonTestAssemblies, true);
            testPackage.AddSetting(FrameworkPackageSettings.DefaultTimeout, config.Timeout);
            testPackage.AddSetting(FrameworkPackageSettings.StopOnError, false);
            return testPackage;
            */
        }
    }
}
