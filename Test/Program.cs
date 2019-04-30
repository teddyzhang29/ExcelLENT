using ExcelParser;
using ExcelParser.Generator;
using ExcelParser.Serializer;
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
                ExcelDir = "../../Excel",
                Logger = new ConsoleLogger(),
                Serializations = new SerializationParam[]
                {
                    new SerializationParam()
                    {
                        Serializer = new TupledJsonSerializer(),
                        OutDir = "../../Output/Serialization",
                    },
                },
                Generations = new GenerationParam[]
                {
                    new GenerationParam()
                    {
                        Generator = new TupledCSharpGenerator(),
                        OutDir = "../../Output/Generation",
                    },
                },
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