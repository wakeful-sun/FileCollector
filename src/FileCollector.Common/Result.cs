namespace FileCollector.Common
{
    public class Result
    {
        public ResultType ResultType { get; }
        public string Message { get; }

        private Result(ResultType resultType, string message = null)
        {
            ResultType = resultType;
            Message = message;
        }

        public static Result Ok() { return new Result(ResultType.Success); }
        public static Result Error(string message) { return new Result(ResultType.Failure, message); }
        public static Result Warning(string message) { return new Result(ResultType.Warning, message); }
    }
}
