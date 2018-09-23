using FileCollector.Common;
using System;

namespace FileCollector.Worker
{
    static class TextExtensions
    {
        public static void PrintLine(this string text)
        {
            Console.WriteLine(text);
        }

        public static void Print(this string text)
        {
            Console.Write(text);
        }

        public static void PrintInColor(this string text, ConsoleColor color)
        {
            DoInColor(text.Print, color);
        }

        public static void PrintLineInColor(this string text, ConsoleColor color)
        {
            DoInColor(text.PrintLine, color);
        }

        static void DoInColor(Action action, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                action();
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        public static void PrintLineInYellow(this string text)
        {
            text.PrintLineInColor(ConsoleColor.Yellow);
        }

        public static void PrintLineInGreen(this string text)
        {
            text.PrintLineInColor(ConsoleColor.Green);
        }

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
