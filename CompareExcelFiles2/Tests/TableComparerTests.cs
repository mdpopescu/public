using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.CompareExcelFiles2.Library;

namespace Renfield.Tests
{
  [TestClass]
  public class TableComparerTests
  {
    [TestClass]
    public class Compare : TableComparerTests
    {
      [TestMethod]
      public void ReturnsAllRowsWhenTheOtherTableIsEmpty()
      {
        var table1 = new MemoryTable(new[] { new[] { "A", "B" }, new[] { "1", "2" }, new[] { "3", "4" } });
        var table2 = new MemoryTable(new[] { new[] { "A", "B" } });
        var sut = new TableComparer(new[] { "A" });

        var result = sut.Compare(table1, table2);

        Assert.AreEqual(2, result.RowCount);
      }

      [TestMethod]
      public void ReturnsRowsThatDifferInOneColumn()
      {
        var table1 = new MemoryTable(new[]
        {
          new[] { "A", "B" },
          new[] { "1", "2" },
          new[] { "3", "4" },
          new[] { "5", "6" },
          new[] { "7", "8" },
        });
        var table2 = new MemoryTable(new[]
        {
          new[] { "A", "B" },
          new[] { "1", "2" },
          new[] { "5", "6" },
        });
        var sut = new TableComparer(new[] { "A" });

        var result = sut.Compare(table1, table2);

        Assert.AreEqual(2, result.RowCount);
        CollectionAssert.AreEqual(new[] { "3", "4" }, result.Data[0]);
        CollectionAssert.AreEqual(new[] { "7", "8" }, result.Data[1]);
      }
    }
  }
}