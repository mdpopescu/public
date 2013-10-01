using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Renfield.BruteForcePuzzle
{
  internal static class Program
  {
    private static void Main()
    {
      Console.Write("Enter a puzzle of the form WORD +/- WORD +/- ... +/- WORD: ");
      var puzzle = Console.ReadLine();
      Console.Write("Enter the desired (numeric) result: ");
      var result = int.Parse(Console.ReadLine());

      var parts = puzzle.Split(new[] {'+', '-', '='}, StringSplitOptions.RemoveEmptyEntries);

      var alphabet = parts.SelectMany(part => part.ToArray()).Distinct().ToArray();
      var letterCount = alphabet.Length;
      if (letterCount > 10)
      {
        Console.WriteLine("Too many letters.");
        return;
      }

      var evaluator = new Evaluator(new Parser(new OperatorFactory(), new OperandFactory()));

      var minValue = int.Parse("0123456789".Left(letterCount));
      var maxValue = int.Parse("9876543210".Left(letterCount));

      var count = 0;
      Console.WriteLine("Solutions:");
      for (var i = minValue; i <= maxValue; i++)
      {
        var digits = i.ToString(CultureInfo.InvariantCulture).ToArray().Distinct().ToArray();
        if (digits.Length < letterCount)
          continue;

        var attempt = ReplaceDigits(puzzle, alphabet, digits);
        var attemptValue = evaluator.Eval(attempt);
        if (attemptValue != result)
          continue;

        DisplaySolution(attempt, alphabet, digits);
        count++;
      }

      Console.WriteLine("{0} solutions found.", count);
    }

    private static string ReplaceDigits(string puzzle, IEnumerable<char> alphabet, IEnumerable<char> digits)
    {
      var map = alphabet.Zip(digits, (letter, digit) => new {letter, digit});

      return map.Aggregate(puzzle, (current, mapping) => current.Replace(mapping.letter, mapping.digit));
    }

    private static void DisplaySolution(string attempt, IEnumerable<char> alphabet, IEnumerable<char> digits)
    {
      Console.WriteLine("Possible solution: " + attempt);
      Console.WriteLine(string.Join(", ", alphabet.Zip(digits, (letter, digit) => letter + "=" + digit)));
    }

    private static string Left(this string s, int count)
    {
      return s.Substring(0, Math.Min(count, s.Length));
    }
  }
}