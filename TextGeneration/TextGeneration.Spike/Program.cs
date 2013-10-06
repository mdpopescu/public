using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.TextGeneration.Spike.Properties;

namespace Renfield.TextGeneration.Spike
{
  internal static class Program
  {
    private const string NONWORD = "";

    private static void Main()
    {
      const int MAXGEN = 1000;

      var corpus = Resources.english;

      var suffixChain = new WordChain();

      var prefix = new Prefix();

      var words = corpus
        .Split(new[] {Environment.NewLine}, StringSplitOptions.None)
        .SelectMany(line => line.Split(' '))
        .ToList();
      foreach (var word in words)
      {
        suffixChain.Add(prefix, word);
        prefix.Add(word);
      }
      suffixChain.Add(prefix, NONWORD);

      var wordChain = Generate(suffixChain, MAXGEN);
      var text = string.Join(" ", wordChain);

      Console.WriteLine(text);
    }

    private static IEnumerable<string> Generate(WordChain suffixChain, int count)
    {
      var prefix = new Prefix();

      var words = Enumerable
        .Range(1, count)
        .Select(_ => suffixChain.Get(prefix))
        .TakeWhile(next => next != NONWORD);
      foreach (var next in words)
      {
        yield return next;
        prefix.Add(next);
      }
    }
  }
}