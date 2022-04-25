namespace Mnk.ParallelTests.Contracts
{
    public interface ITestFacade
    {
        int RunTests(TestsConfig config, string handle);
        int Run(string handle, string path, int[] items, TestsConfig config);
        ResultMessage CollectTests(string path, TestsConfig config);
    }
}