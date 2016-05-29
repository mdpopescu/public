using WebScraping.Library.Implementations.StmtComp;

namespace WebScraping.Library.Implementations
{
    public static class Bootstrapper
    {
        public static Runner Create()
        {
            var compiler = new MultiStepCompiler(new PrintCompiler());
            var interpreter = new CSharpInterpreter();
            return new Runner(compiler, interpreter);
        }
    }
}