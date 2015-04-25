using System.Collections.Generic;
using System.Linq;

namespace TransformyClone.Library
{
  public class TemplateBuilder : Builder
  {
    public string Build(string line, string sample, IEnumerable<string> words)
    {
      sample = DuplicateCurlyBraces(sample);

      var index = 0;
      return words.Aggregate(sample, (current, word) => current.Replace(word, "{" + (index++) + "}"));
    }

    //

    private string DuplicateCurlyBraces(string s)
    {
      return s
        .Replace("{", "{{")
        .Replace("}", "}}");
    }
  }
}