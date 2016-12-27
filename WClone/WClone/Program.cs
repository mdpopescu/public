using System.Linq;
using Jokedst.GetOpt;
using WClone.Library.Implementations;
using WClone.Library.Models;

namespace WClone
{
    internal class Program
    {
        private static readonly Settings SETTINGS = new Settings();

        private static void Main(string[] args)
        {
            var opts = new GetOpt("This application clones a website into a local folder, rewriting URLs as needed.",
                new[]
                {
                    new CommandLineOption('o', "outputPath", "The folder where the downloaded pages will be written",
                        ParameterType.String, value => SETTINGS.OutputPath = (string) value),
                }
            );
            opts.ParseOptions(args);

            var downloader = new WebDownloader();
            if (opts.AdditionalParameters.Any())
                downloader.Download(opts.AdditionalParameters[0], SETTINGS);
        }
    }
}