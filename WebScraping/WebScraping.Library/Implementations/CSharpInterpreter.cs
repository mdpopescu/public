using CSScriptLibrary;
using WebScraping.Library.Interfaces;
using WebScraping.Library.Models;

namespace WebScraping.Library.Implementations
{
    public class CSharpInterpreter : Interpreter
    {
        public void Run(string program, Environment environment)
        {
            var asmFile = CSScript.CompileCode(program, null, false);
            using (var helper = new AsmHelper(asmFile, "tempDomain", true))
            {
                helper.Invoke("Program.Main", environment.Input, environment.Output);
            }
        }
    }
}