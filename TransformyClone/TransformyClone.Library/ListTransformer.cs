using System;
using System.Collections.Generic;
using System.Linq;

namespace TransformyClone.Library
{
  public class ListTransformer
  {
    public IEnumerable<string> Transform(IEnumerable<string> inputs, string sample)
    {
      if (inputs == null)
        throw new ArgumentException("Argument must not be null or empty.", "inputs");
      inputs = inputs.ToList();
      if (!inputs.Any())
        throw new ArgumentException("Argument must not be null or empty.", "inputs");
      if (string.IsNullOrEmpty(sample))
        throw new ArgumentException("String must not be null or empty.", "sample");

      return inputs.Select(_ => sample);
    }
  }
}