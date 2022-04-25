namespace Mnk.ConsoleUnitTestsRunner.Code.Contracts
{
    interface IInfoView
    {
        void ShowArgs(CommandLineArgs cmd);
        void ShowLogo();
        void ShowHelp();
    }
}
