using System.Collections.Generic;
using System.Linq;

namespace TransformyClone.Library
{
  public class TemplateBuilder : Builder
  {
    public string Build(string sample, IEnumerable<string> words)
    {
      sample = sample.DuplicateCurlyBraces();

      var index = 0;
      return words.Aggregate(sample, (current, word) => current.Replace(word, "{" + (index++) + "}"));
    }
  }
}