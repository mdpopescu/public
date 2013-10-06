using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.TextGeneration.Spike
{
  public class Prefix : IEquatable<Prefix>
  {
    public const string NONWORD = "";

    public Prefix()
    {
      words = Enumerable
        .Range(1, LENGTH)
        .Select(_ => NONWORD)
        .ToList();
    }

    public override string ToString()
    {
      return "(" + string.Join(", ", words) + ")";
    }

    public Prefix Copy()
    {
      var copy = new Prefix();
      foreach (var word in words)
        copy.Add(word);

      return copy;
    }

    public void Add(string word)
    {
      words.RemoveAt(0);
      words.Add(word);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return words.Aggregate(17, (acc, word) => acc * 23 + word.GetHashCode());
      }
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as Prefix);
    }

    public bool Equals(Prefix obj)
    {
      return obj != null && obj.words.SequenceEqual(words);
    }

    //

    private const int LENGTH = 3;

    private readonly List<string> words;
  }
}