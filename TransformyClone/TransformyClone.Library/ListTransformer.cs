using System;
using System.Collections.Generic;
using System.Linq;

namespace TransformyClone.Library
{
  public class ListTransformer
  {
    public List<string> Transform(List<string> inputs, string sample)
    {
      if (inputs == null || !inputs.Any())
        throw new ArgumentException("Argument must not be null or empty.", "inputs");
      if (string.IsNullOrEmpty(sample))
        throw new ArgumentException("String must not be null or empty.", "sample");

      return new List<string>();
    }
  }
}