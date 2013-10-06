using System;
using System.Collections.Generic;

namespace Renfield.TextGeneration.Spike
{
  public class WordChain
  {
    public WordChain()
    {
      chain = new Dictionary<Prefix, List<string>>();
      rnd = new Random();
    }

    public void Add(Prefix prefix, string word)
    {
      prefix = prefix.Copy();

      if (!chain.ContainsKey(prefix))
        chain.Add(prefix, new List<string>());
      chain[prefix].Add(word);
    }

    public string Get(Prefix prefix)
    {
      var suffixes = chain[prefix];
      var index = rnd.Next(suffixes.Count);

      return suffixes[index];
    }

    //

    private readonly Dictionary<Prefix, List<string>> chain;
    private readonly Random rnd;
  }
}