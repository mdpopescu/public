using System;
using System.Diagnostics;
using CustomCrypto.Library.Implementations;

namespace CustomCrypto.Tester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Test();
            BenchmarkEncrypt();
            BenchmarkDecrypt();
        }

        //

        private const string KEY = "some key";

        private const int COUNT = 100000;

        private static void Test()
        {
            do
            {
                Console.Write("Enter the message to encrypt:");
                var message = Console.ReadLine();
                if (string.IsNullOrEmpty(message))
                    break;

                var encrypted = Encrypt(message);
                Console.WriteLine($"{encrypted.Length} words:");
                foreach (var it in encrypted)
                    Console.Write("{0:X8}  ", it);
                Console.WriteLine();

                var decrypted = Decrypt(encrypted);

                Console.WriteLine($"The decrypted message is [{decrypted}]");
            } while (true);
        }

        private static void BenchmarkEncrypt()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 1; i <= COUNT; i++)
                Encrypt("some message");

            sw.Stop();

            Console.WriteLine($"Time spent to encrypt a message {COUNT} times: {sw.Elapsed}");
        }

        private static void BenchmarkDecrypt()
        {
            var encrypted = Encrypt("some message");

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 1; i <= COUNT; i++)
                Decrypt(encrypted);

            sw.Stop();

            Console.WriteLine($"Time spent to decrypt a message {COUNT} times: {sw.Elapsed}");
        }

        private static uint[] Encrypt(string message)
        {
            var encryptor = new Encryptor(KEY);
            return encryptor.Encrypt(message);
        }

        private static string Decrypt(uint[] encrypted)
        {
            var encryptor = new Encryptor(KEY);
            return encryptor.Decrypt(encrypted);
        }
    }
}