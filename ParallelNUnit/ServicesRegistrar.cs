using LightInject;
using Mnk.Library.Common.Tools;
using Mnk.ParallelTests.Contracts;
using Mnk.ParallelTests.Common;
using Mnk.ParallelNUnit.Core;
using Mnk.ParallelNUnit.Packages;
using Mnk.ParallelNUnit.Packages.Common;
using Mnk.ParallelNUnit.Packages.Execution;

namespace Mnk.ParallelNUnit
{
    public static class ServicesRegistrar
    {
        public static void Register(IServiceContainer container)
        {
            container.Register<ITestFacade, NUnitTestFacade>(new PerContainerLifetime());
            container.Register<ICopyDirGenerator, CopyDirGenerator>(new PerContainerLifetime());
            container.Register<ITestsDivider, TestsDivider>(new PerContainerLifetime());
            container.Register<ITestsExecutor, TestsExecutor>(new PerContainerLifetime());
            container.Register<ITestsMetricsCalculator, TestsMetricsCalculator>(new PerContainerLifetime());
            container.Register<IDirectoriesManipulator, DirectoriesManipulator>(new PerContainerLifetime());
            container.Register<IOrderOptimizationManager, OrderOptimizationManager>(new PerContainerLifetime());

            container.Register<ITestsExecutionFacade, InternalTestsExecutionFacade>(TestsRunnerMode.Internal.ToLower(), new PerContainerLifetime());
            container.Register<ITestsExecutionFacade, ProcessTestsExecutionFacade>(TestsRunnerMode.Process.ToLower(), new PerContainerLifetime());
            container.Register<ITestsExecutionFacade, MultiProcessTestsExecutionFacade>(TestsRunnerMode.MultiProcess.ToLower(), new PerContainerLifetime());

            container.RegisterInstance(typeof(Func<string, ITestsExecutionFacade>),
                new Func<string, ITestsExecutionFacade>(name => container.GetInstance<ITestsExecutionFacade>(name.ToLower())));

            container.Register<ITestsFixture, TestsFixture>(new PerContainerLifetime());
            container.Register<IMultiTestsFixture, MultiTestsFixture>(new PerContainerLifetime());
        }

        public static IServiceContainer Register()
        {
            var container = new ServiceContainer();

            Register(container);

            return container;
        }
    }
}
