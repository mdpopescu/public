using System;

namespace Renfield.FSM.ParseNumbers
{
    internal class Program
    {
        private static FSMParser parser;

        private static void Main(string[] args)
        {
            parser = new FSMParser();
            Verify();

            do
            {
                Console.Write("Enter a number: ");
                var s = Console.ReadLine();
                if (string.IsNullOrEmpty(s))
                    break;

                try
                {
                    Console.WriteLine($"The number is {parser.Parse(s)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } while (true);
        }

        private static void Verify()
        {
            AssertEquals(0.0, "");
            AssertEquals(0.0, "0");
            AssertEquals(0.0, "-0");
            AssertEquals(0.0, "+0");
            AssertEquals(0.0, "-00.00");
            AssertEquals(0.0, "+00.00");
            AssertEquals(0.1, ".1");
            AssertEquals(0.1, "0.1");
            AssertEquals(0.1, "00.1");
            AssertEquals(0.1, "000.100");
            AssertEquals(-0.1, "-.1");
            AssertEquals(-0.1, "-0.1");
            AssertEquals(-0.1, "-00.10");
            AssertEquals(-12.34, "-12.34");
            AssertEquals(12.34, "12.34");
            AssertEquals(12.34, "+12.34");
            AssertEquals(12.34, "+012.340");
        }

        private static void AssertEquals(double expected, string s)
        {
            var actual = parser.Parse(s);
            if (Math.Abs(expected - actual) < 0.00001)
                Console.WriteLine($"[{s}] ok");
            else
                throw new Exception($"Assertion failed: parsing {s} resulted in {actual} but was expecting {expected}");
        }
    }
}