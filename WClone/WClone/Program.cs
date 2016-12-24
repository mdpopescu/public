using Jokedst.GetOpt;
using WClone.Models;

namespace WClone
{
    internal class Program
    {
        private static readonly Options OPTIONS = new Options();

        private static void Main(string[] args)
        {
            var opts = new GetOpt("This application clones a website into a local folder, rewriting URLs as needed.",
                new[]
                {
                    new CommandLineOption('d', "maxDepth", "The maximum depth to download (default: unlimited)",
                        ParameterType.Integer, value => OPTIONS.MaxDepth = (int) value),
                }
            );
            opts.ParseOptions(args);
        }
    }
}