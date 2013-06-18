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

      [TestMethod]
      public void SortsFile1()
      {
        var columns = new[] { "A", "B", "C", "D" };
        var rows = new[]
        {
          new[] { "1", "10", "1", "100", },
          new[] { "2", "20", "1", "200", },
          new[] { "3", "30", "1", "300", },
          new[] { "1", "40", "2", "400", },
          new[] { "2", "50", "2", "500", },
          new[] { "3", "60", "2", "600", },
          new[] { "1", "70", "3", "700", },
          new[] { "2", "80", "3", "800", },
          new[] { "3", "90", "3", "900", },
        };
        var sut = new Table(columns, new RowList(rows));

        sut.SortBy(new[] { "C", "A" });

        var lines = new List<string>();
        sut.Dump(columns, lines.Add);
        Assert.AreEqual(10, lines.Count);
        Assert.AreEqual("      A B C D", lines[0]);
        Assert.AreEqual("00001 1 10 1 100", lines[1]);
        Assert.AreEqual("00002 2 20 1 200", lines[2]);
        Assert.AreEqual("00003 3 30 1 300", lines[3]);
        Assert.AreEqual("00004 1 40 2 400", lines[4]);
        Assert.AreEqual("00005 2 50 2 500", lines[5]);
        Assert.AreEqual("00006 3 60 2 600", lines[6]);
        Assert.AreEqual("00007 1 70 3 700", lines[7]);
        Assert.AreEqual("00008 2 80 3 800", lines[8]);
        Assert.AreEqual("00009 3 90 3 900", lines[9]);
      }

      [TestMethod]
      public void SortsFile2()
      {
        var columns = new[] { "A", "B", "C", "D" };
        var rows = new[]
        {
          new[] { "1", "5000", "3", "1400", },
          new[] { "2", "6000", "3", "1500", },
          new[] { "3", "7000", "3", "1600", },
          new[] { "4", "8000", "3", "1700", },
          new[] { "1", "1000", "1", "1000", },
          new[] { "2", "2000", "1", "1100", },
          new[] { "3", "3000", "1", "1200", },
          new[] { "4", "4000", "1", "1300", },
        };
        var sut = new Table(columns, new RowList(rows));

        sut.SortBy(new[] { "C", "A" });

        var lines = new List<string>();
        sut.Dump(columns, lines.Add);
        Assert.AreEqual(9, lines.Count);
        Assert.AreEqual("      A B C D", lines[0]);
        Assert.AreEqual("00001 1 1000 1 1000", lines[1]);
        Assert.AreEqual("00002 2 2000 1 1100", lines[2]);
        Assert.AreEqual("00003 3 3000 1 1200", lines[3]);
        Assert.AreEqual("00004 4 4000 1 1300", lines[4]);
        Assert.AreEqual("00005 1 5000 3 1400", lines[5]);
        Assert.AreEqual("00006 2 6000 3 1500", lines[6]);
        Assert.AreEqual("00007 3 7000 3 1600", lines[7]);
        Assert.AreEqual("00008 4 8000 3 1700", lines[8]);
      }
    }

    [TestClass]
    public class ExcludeRecords : TableTests
    {
      [TestMethod]
      public void File1Minus2()
      {
        var columns = new[] { "A", "B", "C", "D" };
        var rows1 = new[]
        {
          new[] { "1", "10", "1", "100", },
          new[] { "2", "20", "1", "200", },
          new[] { "3", "30", "1", "300", },
          new[] { "1", "40", "2", "400", },
          new[] { "2", "50", "2", "500", },
          new[] { "3", "60", "2", "600", },
          new[] { "1", "70", "3", "700", },
          new[] { "2", "80", "3", "800", },
          new[] { "3", "90", "3", "900", },
        };
        var sut = new Table(columns, new RowList(rows1));
        sut.SortBy(new[] { "C", "A" });
        var rows2 = new[]
        {
          new[] { "1", "5000", "3", "1400", },
          new[] { "2", "6000", "3", "1500", },
          new[] { "3", "7000", "3", "1600", },
          new[] { "4", "8000", "3", "1700", },
          new[] { "1", "1000", "1", "1000", },
          new[] { "2", "2000", "1", "1100", },
          new[] { "3", "3000", "1", "1200", },
          new[] { "4", "4000", "1", "1300", },
        };
        var other = new Table(columns, new RowList(rows2));
        other.SortBy(new[] { "C", "A" });

        var result = sut.ExcludeRecords(other, new[] { "C", "A" });

        var lines = new List<string>();
        result.Dump(columns, lines.Add);
        Assert.AreEqual(4, lines.Count);
        Assert.AreEqual("00001 1 40 2 400", lines[1]);
        Assert.AreEqual("00002 2 50 2 500", lines[2]);
        Assert.AreEqual("00003 3 60 2 600", lines[3]);
      }
    }
  }
}