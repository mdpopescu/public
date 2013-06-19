using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.CompareExcelFiles2.Library;

namespace Renfield.Tests
{
  [TestClass]
  public class MemoryTableTests
  {
    [TestClass]
    public class RowCount : MemoryTableTests
    {
      [TestMethod]
      public void SingleRow()
      {
        var sut = new MemoryTable(new[]
        {
          new[] { "A", "B" },
          new[] { "1", "2" },
        });

        Assert.AreEqual(1, sut.RowCount);
      }

      [TestMethod]
      public void MultipleRows()
      {
        var sut = new MemoryTable(new[]
        {
          new[] { "A", "B" },
          new[] { "1", "2" },
          new[] { "3", "4" },
          new[] { "5", "6" },
        });

        Assert.AreEqual(3, sut.RowCount);
      }

      [TestMethod]
      [ExpectedException(typeof (Exception))]
      public void ThrowsWhenNoRows()
      {
        var sut = new MemoryTable(new List<string[]>());
      }

      [TestMethod]
      [ExpectedException(typeof (Exception))]
      public void ThrowsWhenNull()
      {
        var sut = new MemoryTable(null);
      }
    }

    [TestClass]
    public class ColCount : MemoryTableTests
    {
      [TestMethod]
      public void SingleColumn()
      {
        var sut = new MemoryTable(new[]
        {
          new[] { "A" },
          new[] { "1" },
          new[] { "2" },
        });

        Assert.AreEqual(1, sut.ColCount);
      }

      [TestMethod]
      public void MultipleColumns()
      {
        var sut = new MemoryTable(new[]
        {
          new[] { "A", "B", "C" },
          new[] { "1", "2", "3" },
          new[] { "4", "5", "6" },
        });

        Assert.AreEqual(3, sut.ColCount);
      }

      [TestMethod]
      [ExpectedException(typeof (Exception))]
      public void ThrowsWhenNoColumns()
      {
        var sut = new MemoryTable(new[]
        {
          new string[0],
          new string[0],
        });
      }
    }

    [TestClass]
    public class Columns : MemoryTableTests
    {
      [TestMethod]
      public void SingleColumn()
      {
        var sut = new MemoryTable(new[]
        {
          new[] { "A" },
          new[] { "1" },
          new[] { "2" },
        });

        CollectionAssert.AreEqual(new[] { "A" }, sut.Columns);
      }

      [TestMethod]
      public void MultipleColumns()
      {
        var sut = new MemoryTable(new[]
        {
          new[] { "A", "B", "C" },
          new[] { "1", "2", "3" },
          new[] { "4", "5", "6" },
        });

        CollectionAssert.AreEqual(new[] { "A", "B", "C" }, sut.Columns);
      }
    }
  }
}