using System;
using System.Text;
using Secure.Services;

namespace Secure
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Command-line Encryption / Decryption Utility -- using AES");
                Console.WriteLine();
                Console.WriteLine("USAGE: secure enc[rypt] {password} -- encrypts STDIN and writes the result to STDOUT (Base64-encoded).");
                Console.WriteLine("       secure dec[rypt] {password} -- decodes (Base64) and decrypts STDIN and writes the result to STDOUT.");
                Console.WriteLine();
                Console.WriteLine("Note:  if using STDIN for testing, use Ctrl-Z to end the input.");

                return;
            }

            var factory = new TransformerFactory();

            var transformer = factory.Create(args[0]);
            var result = transformer.Transform(args[1], ReadInput());
            Console.WriteLine(result);
        }

        private static string ReadInput()
        {
            var sb = new StringBuilder();

            string line;
            while ((line = Console.ReadLine()) != null)
                sb.AppendLine(line);

            return sb.ToString();
        }
    }
}