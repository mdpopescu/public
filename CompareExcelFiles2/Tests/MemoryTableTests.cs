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
    }
  }
}