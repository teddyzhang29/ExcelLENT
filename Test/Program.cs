using ExcelParser;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            parser.Parse(new ParseParam()
            {
                ExcelDir = "../../../Excel",
                OutputDir = "../../../Output",
                Logger = new ConsoleLogger(),
            });
        }
    }

    class ConsoleLogger : ILogger
    {
        public void Log(object msg)
        {
            Console.WriteLine(msg);
        }

        public void LogError(object msg)
        {
            ConsoleColor savedColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = savedColor;
        }

        public void LogWarning(object msg)
        {
            ConsoleColor savedColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ForegroundColor = savedColor;
        }
    }
}