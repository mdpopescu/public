using System.Collections.Generic;

namespace TransformyClone.Library
{
  public interface Splitter
  {
    IEnumerable<string> Split(string s);
  }
}