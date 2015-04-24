using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransformyClone.Library;
using TransformyClone.Tests.Properties;

namespace TransformyClone.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [Ignore]
    [TestMethod]
    public void TestUsingSamples()
    {
      TestUsingResource(Resources.Sample1);
      TestUsingResource(Resources.Sample2);
      TestUsingResource(Resources.Sample3);
    }

    //

    private static void TestUsingResource(string value)
    {
      List<string> inputs, outputs;
      string sample;

      Extract(value, out inputs, out sample, out outputs);
      var sut = new ListTransformer();

      var result = sut.Transform(inputs, sample);

      CollectionAssert.AreEqual(outputs.ToArray(), result.ToArray());
    }

    private static void Extract(string value, out List<string> inputs, out string sample, out List<string> outputs)
    {
      var lines = value
        .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
        .ToList();
      var emptyIndices = lines
        .Select((line, index) => new { line, index })
        .Where(it => string.IsNullOrWhiteSpace(it.line))
        .Take(2)
        .Select(it => it.index)
        .ToList();

      Assert.AreEqual(2, emptyIndices.Count);

      inputs = lines.Take(emptyIndices[0]).ToList();
      sample = lines[emptyIndices[0] + 1];
      outputs = lines.Skip(emptyIndices[1] + 1).Where(line => !string.IsNullOrEmpty(line)).ToList();
    }
  }
}