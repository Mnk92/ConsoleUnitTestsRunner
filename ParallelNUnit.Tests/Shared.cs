namespace Mnk.ParallelNUnit.Tests
{
    static class Shared
    {
        public const string CompileMode =
#if DEBUG
            "DEBUG";
#else
 "Release";
#endif

    }
}