using LightInject;
using Mnk.Library.Common.MT;
using Mnk.ConsoleUnitTestsRunner.Code.Contracts;
using Mnk.ParallelTests.Common;
using Mnk.ParallelTests.Contracts;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    internal static class ServicesRegistrar
    {
        public static IServiceContainer Register()
        {
            var container = new ServiceContainer();
            container.Register<IConsoleView, ConsoleView>(new PerContainerLifetime());
            container.Register<ITestsSummaryBuilder, TestsSummaryBuilder>(new PerContainerLifetime());
            container.Register<IInfoView, InfoView>(new PerContainerLifetime());
            container.Register<IUpdater, ConsoleUpdater>(new PerContainerLifetime());
            container.Register<IReportBuilder, ReportBuilder>(new PerContainerLifetime());
            container.Register<IConsoleTestsExecutor, ConsoleTestsExecutor>(new PerContainerLifetime());
            container.Register<IExecutor, Executor>(new PerContainerLifetime());
            ParallelNUnit.ServicesRegistrar.Register(container);
            return container;
        }
    }
}
