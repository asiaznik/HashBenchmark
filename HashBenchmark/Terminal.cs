namespace HashBenchmark
{
    using System;

    public static class Terminal
    {
        private static ConsoleColor defaultColor = ConsoleColor.White;

        public static void Log(string input)
        {
            Write(input, ConsoleColor.Gray);
        }

        public static void Error(string input)
        {
            Write(input, ConsoleColor.Red);
        }

        public static void Info(string input)
        {
            Write(input, ConsoleColor.Cyan);
        }

        public static void Success(string input)
        {
            Write(input, ConsoleColor.Green);
        }

        public static void Waring(string input)
        {
            Write(input, ConsoleColor.Yellow);
        }

        public static void Write(string input)
        {
            Write(input, defaultColor);
        }

        public static void Write(string input, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(input);
            Console.ForegroundColor = defaultColor;
        }

        public static void CleanLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
