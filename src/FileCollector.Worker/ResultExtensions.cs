using FileCollector.Common;
using System;

namespace FileCollector.Worker
{
    static class ResultExtensions
    {
        public static void Print(this Result result)
        {
            switch (result.ResultType)
            {
                case ResultType.Failure:
                    $"\t [FAILED] {Environment.NewLine} {result.Message}".PrintInColor(ConsoleColor.Red);
                    break;
                case ResultType.Success:
                    "\t [FINISHED]".PrintInColor(ConsoleColor.DarkGreen);
                    break;
                case ResultType.Warning:
                    $"\t [WARNING] {Environment.NewLine} {result.Message}".PrintInColor(ConsoleColor.Yellow);
                    break;
                default:
                    throw new NotImplementedException($"No implementation found for {result.ResultType} result.");
            }
            "".PrintLine();
        }
    }
}
