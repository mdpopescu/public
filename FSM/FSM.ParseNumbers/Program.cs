using System;
using Renfield.FSM.Library;

namespace Renfield.FSM.ParseNumbers
{
  internal class Program
  {
    private static FSM<char> fsm;
    private static int sign;
    private static double number;
    private static double divisor;

    private static void Main(string[] args)
    {
      fsm = new FSM<char>("start");

      fsm.Add("start", '+', null, "whole");
      fsm.Add("start", '-', c => sign = -1, "whole");
      fsm.Add("start", char.IsDigit, AddChar, "whole");
      fsm.Add("start", '.', c => divisor = 0.1, "fract");

      fsm.Add("whole", char.IsDigit, AddChar);
      fsm.Add("whole", '.', c => divisor = 0.1, "fract");

      fsm.Add("fract", char.IsDigit, c =>
      {
        number += divisor * (c - '0');
        divisor /= 10;
      });

      fsm.OnStart = () =>
      {
        sign = 1;
        number = 0.0;
        divisor = 0.0;
      };

      Verify();

      do
      {
        Console.Write("Enter a number: ");
        var s = Console.ReadLine();
        if (string.IsNullOrEmpty(s))
          break;

        fsm.Restart();
        foreach (var c in s)
          fsm.Handle(c);

        Console.WriteLine("The number is " + sign * number);
      } while (true);
    }

    private static void AddChar(char c)
    {
      number = number * 10 + (c - '0');
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

    private static void AssertEquals(double result, string s)
    {
      fsm.Restart();

      foreach (var c in s)
        fsm.Handle(c);

      number *= sign;
      if (Math.Abs(result - number) < 0.00001)
        Console.WriteLine("[{0}] ok", s);
      else
        throw new Exception(string.Format("Assertion failed: parsing {0} resulted in {1} but was expecting {2}", s,
          number, result));
    }
  }
}