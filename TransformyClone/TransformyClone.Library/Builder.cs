using System.Collections.Generic;

namespace TransformyClone.Library
{
  public interface Builder
  {
    string Build(string sample, IEnumerable<string> words);
  }
}