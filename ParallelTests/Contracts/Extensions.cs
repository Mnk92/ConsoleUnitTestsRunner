namespace Mnk.ParallelTests.Contracts
{
    public static class Extensions
    {
        public static bool IsSuccess(this ResultMessage result)
        {
            return result.State == ResultMessage.Types.TestResultState.Success;
        }

        public static bool IsInconclusive(this ResultMessage result)
        {
            return result.State == ResultMessage.Types.TestResultState.Inconclusive;
        }

        public static bool IsFailure(this ResultMessage result)
        {
            return result.State == ResultMessage.Types.TestResultState.Failure;
        }

        public static bool IsError(this ResultMessage result)
        {
            return result.State == ResultMessage.Types.TestResultState.Error;
        }
        public static bool IsIgnored(this ResultMessage result)
        {
            return result.State == ResultMessage.Types.TestResultState.Ignored;
        }
        public static bool IsSkipped(this ResultMessage result)
        {
            return result.State == ResultMessage.Types.TestResultState.Skipped;
        }

        public static bool IsFailed(this ResultMessage result)
        {
            return IsFailure(result) || IsError(result);
        }
        public static bool IsTest(this ResultMessage result)
        {
            return string.Equals(result.Type, "TestMethod");
        }
        public static bool Executed(this ResultMessage result)
        {
            return IsSuccess(result) || IsInconclusive(result) || IsFailed(result);
        }
        public static bool HasResults(this ResultMessage result)
        {
            return result.Children is { Count: > 0 };
        }
        public static IEnumerable<ResultMessage> Collect(this ResultMessage result)
        {
            foreach (var r in result.Children.SelectMany(Collect))
            {
                yield return r;
            }
            yield return result;
        }
    }
}
