using WebScraping.Library.Interfaces;
using WebScraping.Library.Models;

namespace WebScraping.Library.Implementations
{
    public class Runner
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="Runner" /> class.
        /// </summary>
        /// <param name="compiler">The scraping language to C# compiler.</param>
        /// <param name="interpreter">The C# interpreter.</param>
        public Runner(Compiler compiler, Interpreter interpreter)
        {
            this.compiler = compiler;
            this.interpreter = interpreter;
        }

        /// <summary>
        ///     Runs a program.
        /// </summary>
        /// <param name="program">The program to run.</param>
        /// <param name="environment">The environment needed for the program to run, like input and output.</param>
        public void Run(string program, Environment environment)
        {
            var compiled = compiler.Compile(program);
            interpreter.Run(compiled, environment);
        }

        //

        private readonly Compiler compiler;
        private readonly Interpreter interpreter;
    }
}