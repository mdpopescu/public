using System;
using System.IO;
using DecoratorGen.Library.Services;
using DecoratorGen.Services;

namespace DecoratorGen
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: DecoratorGen <path-to-interface.cs>");
                Console.WriteLine();
                Console.WriteLine("       This will parse the interface, generate a decorator class for it and place the resulting code on the clipboard.");

                return;
            }

            var filename = args[0];

            var path = Path.GetDirectoryName(filename);
            var file = Path.GetFileName(filename);

            var fs = new FileSystem(path);
            var output = new ClipboardOutput();
            var codeGenerator = new CodeGenerator(
                new Parser(),
                new MemberGenerator(),
                new ClassNameGenerator()
            );
            var app = new App(fs, codeGenerator, output);

            app.GenerateDecorator(file);
        }
    }
}