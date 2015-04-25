using System;
using System.Collections.Generic;
using System.Linq;

namespace TransformyClone.Library
{
  public class ListTransformer
  {
    public ListTransformer(Splitter splitter, Builder builder)
    {
      this.splitter = splitter;
      this.builder = builder;
    }

    public IEnumerable<string> Transform(IEnumerable<string> inputs, string sample)
    {
      if (inputs == null)
        throw new ArgumentException("Argument must not be null or empty.", "inputs");
      var list = inputs.ToList();
      if (!list.Any())
        throw new ArgumentException("Argument must not be null or empty.", "inputs");
      if (string.IsNullOrEmpty(sample))
        throw new ArgumentException("String must not be null or empty.", "sample");

      var firstLine = list[0];
      var words = splitter.Split(firstLine);
      var template = builder.Build(firstLine, sample, words);

      // ReSharper disable once CoVariantArrayConversion
      return list.Select(line => string.Format(template, splitter.Split(line).ToArray()));
    }

    //

    private readonly Splitter splitter;
    private readonly Builder builder;
  }
}