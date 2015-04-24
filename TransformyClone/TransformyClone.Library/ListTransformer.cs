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
      var list = inputs.ToList();
      if (!list.Any())
        throw new ArgumentException("Argument must not be null or empty.", "inputs");
      if (string.IsNullOrEmpty(sample))
        throw new ArgumentException("String must not be null or empty.", "sample");

      sample = sample.Replace(list[0], "{0}");

      return list.Select(it => string.Format(sample, it));
    }
  }
}