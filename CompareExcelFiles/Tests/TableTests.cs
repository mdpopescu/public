using System.Collections.Generic;
using Brownstone.CompareExcelFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
  [TestClass]
  public class TableTests
  {
    [TestClass]
    public class SortBy : TableTests
    {
      [TestMethod]
      public void SortsBySingleColumn()
      {
        var columns = new[] { "A", "B" };
        var rows = new[]
        {
          new[] { "3", "a", },
          new[] { "2", "b", },
          new[] { "1", "c", },
        };
        var sut = new Table(columns, new RowList(rows));

        sut.SortBy(new[] { "A" });

        var lines = new List<string>();
        sut.Dump(columns, lines.Add);
        Assert.AreEqual(4, lines.Count);
        Assert.AreEqual("      A B", lines[0]);
        Assert.AreEqual("00001 1 c", lines[1]);
        Assert.AreEqual("00002 2 b", lines[2]);
        Assert.AreEqual("00003 3 a", lines[3]);
      }

      [TestMethod]
      public void SortsByTwoColumns()
      {
        var columns = new[] { "A", "B", "C" };
        var rows = new[]
        {
          new[] { "2", "a", "30", },
          new[] { "1", "b", "10", },
          new[] { "1", "c", "20", },
        };
        var sut = new Table(columns, new RowList(rows));

        sut.SortBy(new[] { "A", "C" });

        var lines = new List<string>();
        sut.Dump(columns, lines.Add);
        Assert.AreEqual(4, lines.Count);
        Assert.AreEqual("      A B C", lines[0]);
        Assert.AreEqual("00001 1 b 10", lines[1]);
        Assert.AreEqual("00002 1 c 20", lines[2]);
        Assert.AreEqual("00003 2 a 30", lines[3]);
      }
    }
  }
}