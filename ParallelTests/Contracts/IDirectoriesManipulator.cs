namespace Mnk.ParallelTests.Contracts
{
    public interface IDirectoriesManipulator
    {
        IList<string> GenerateFolders(TestsConfig config, ITestsUpdater updater, int count);
        void ClearFolders(TestsConfig config, IList<string> dllPaths);
    }
}
