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
      var list = (inputs ?? Enumerable.Empty<string>()).ToList();
      if (!list.Any())
        throw new ArgumentException("Argument must not be null or empty.", "inputs");
      if (string.IsNullOrEmpty(sample))
        throw new ArgumentException("String must not be null or empty.", "sample");

      var template = GetTemplate(list[0], sample);

      return list
        .Select(line => splitter.Split(line).Select(it => (object) it).ToArray())
        .Select(parts => string.Format(template, parts));
    }

    private string GetTemplate(string firstLine, string sample)
    {
      var words = splitter.Split(firstLine);
      return builder.Build(sample, words);
    }

    //

    private readonly Splitter splitter;
    private readonly Builder builder;
  }
}