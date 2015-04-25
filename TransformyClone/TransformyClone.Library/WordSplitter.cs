using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TransformyClone.Library
{
  public class WordSplitter: Splitter
  {
    public IEnumerable<string> Split(string s)
    {
      return Regex
        .Split(s + "", @"\b")
        .Select(it => it.Trim())
        .Where(it => it != "");
    }
  }
}