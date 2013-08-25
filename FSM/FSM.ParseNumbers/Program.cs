using System;
using Renfield.FSM.Library;

namespace Renfield.FSM.ParseNumbers
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var sign = 1;
      var number = 0.0;
      var divisor = 0.0;

      var start = new Node<char>("start");
      var wholePart = new Node<char>("int part");
      var fractPart = new Node<char>("decimal part");

      start.Add('+', null, wholePart);
      start.Add('-', c => sign = -1, wholePart);
      start.Add(char.IsDigit, c => number = number * 10 + (c - '0'), wholePart);
      start.Add('.', c => divisor = 0.1, fractPart);

      wholePart.Add(char.IsDigit, c => number = number * 10 + (c - '0'));
      wholePart.Add('.', c => divisor = 0.1, fractPart);

      fractPart.Add(char.IsDigit, c =>
      {
        number += divisor * (c - '0');
        divisor /= 10;
      });

      do
      {
        sign = 1;
        number = 0.0;
        divisor = 0.0;

        Console.Write("Enter a number: ");
        var s = Console.ReadLine();
        if (string.IsNullOrEmpty(s))
          break;

        var fsm = new FSM<char>(start);
        foreach (var c in s)
          fsm.Handle(c);

        Console.WriteLine("The number is " + sign * number);
      } while (true);
    }
  }
}