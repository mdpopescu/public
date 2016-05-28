using WebScraping.Library.Models;

namespace WebScraping.Library.Interfaces
{
    public interface Interpreter
    {
        void Run(string program, Environment environment);
    }
}