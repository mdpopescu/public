using System;
using System.Diagnostics;
using Elomen.Spike.Contracts;

namespace Elomen.Spike.Implementations
{
    public class ConsoleApprover : Authorizable
    {
        public string Authorize(string url)
        {
            Process.Start(url);

            Console.Write("Enter PIN: ");
            return Console.ReadLine();
        }
    }
}