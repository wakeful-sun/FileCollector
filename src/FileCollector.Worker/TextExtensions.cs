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

        public static void PrintLineInYellow(this string text)
        {
            text.PrintLineInColor(ConsoleColor.Yellow);
        }

        public static void PrintLineInGreen(this string text)
        {
            text.PrintLineInColor(ConsoleColor.Green);
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
    }
}
