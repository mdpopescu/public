using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.RecursiveCompare.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void SimpleType()
    {
      var obj1 = new TestObject1 { Prop1 = "abc", Prop2 = 123 };
      var obj2 = new TestObject1 { Prop1 = "abc", Prop2 = 345 };
      var sut = new ObjectComparer();

      var result = sut.Compare(obj1, obj2).ToList();

      Assert.AreEqual(2, result.Count());
      CheckComparison(result[0], ComparisonResult.Equal, ".Prop1", "abc", "abc");
      CheckComparison(result[1], ComparisonResult.NotEqual, ".Prop2", 123, 345);
    }

    [TestMethod]
    public void ComplexType()
    {
      var obj1 = new TestObject2 { PropX = "xyz", PropY = new TestObject1 { Prop1 = "abc", Prop2 = 123 } };
      var obj2 = new TestObject2 { PropX = "zyx", PropY = new TestObject1 { Prop1 = "abc", Prop2 = 345 } };
      var sut = new ObjectComparer();

      var result = sut.Compare(obj1, obj2).ToList();

      Assert.AreEqual(3, result.Count());
      CheckComparison(result[0], ComparisonResult.NotEqual, ".PropX", "xyz", "zyx");
      CheckComparison(result[1], ComparisonResult.Equal, ".PropY.Prop1", "abc", "abc");
      CheckComparison(result[2], ComparisonResult.NotEqual, ".PropY.Prop2", 123, 345);
    }

    [TestMethod]
    public void ComplexTypeWithArrayProperties()
    {
      var obj1 = new TestObject3
      {
        List = new[]
        {
          new TestObject2 { PropX = "xyz", PropY = new TestObject1 { Prop1 = "abc", Prop2 = 123 } },
          new TestObject2 { PropX = "xyz", PropY = new TestObject1 { Prop1 = "abc", Prop2 = 123 } },
        }
      };
      var obj2 = new TestObject3
      {
        List = new[]
        {
          new TestObject2 { PropX = "abc", PropY = new TestObject1 { Prop1 = "x1", Prop2 = 123 } },
          new TestObject2 { PropX = "def", PropY = new TestObject1 { Prop1 = "x2", Prop2 = 456 } },
          new TestObject2 { PropX = "ghi", PropY = new TestObject1 { Prop1 = "x3", Prop2 = 789 } },
        }
      };
      var sut = new ObjectComparer();

      var result = sut.Compare(obj1, obj2).ToList();

      Assert.AreEqual(10, result.Count());
      CheckComparison(result[0], ComparisonResult.NotEqual, ".List.Length", 2, 3);
      CheckComparison(result[1], ComparisonResult.NotEqual, ".List[0].PropX", "xyz", "abc");
      CheckComparison(result[2], ComparisonResult.NotEqual, ".List[0].PropY.Prop1", "abc", "x1");
      CheckComparison(result[3], ComparisonResult.Equal, ".List[0].PropY.Prop2", 123, 123);
      CheckComparison(result[4], ComparisonResult.NotEqual, ".List[1].PropX", "xyz", "def");
      CheckComparison(result[5], ComparisonResult.NotEqual, ".List[1].PropY.Prop1", "abc", "x2");
      CheckComparison(result[6], ComparisonResult.NotEqual, ".List[1].PropY.Prop2", 123, 456);
      CheckComparison(result[7], ComparisonResult.NotEqual, ".List[2].PropX", null, "ghi");
      CheckComparison(result[8], ComparisonResult.NotEqual, ".List[2].PropY.Prop1", null, "x3");
      CheckComparison(result[9], ComparisonResult.NotEqual, ".List[2].PropY.Prop2", null, 789);
    }

    //

    private static void CheckComparison(Comparison comparison, ComparisonResult comparisonResult, string propName, object lValue, object rValue)
    {
      Assert.AreEqual(comparisonResult, comparison.Result);
      Assert.AreEqual(propName, comparison.PropName);
      Assert.AreEqual(lValue, comparison.LValue);
      Assert.AreEqual(rValue, comparison.RValue);
    }

    //

    private class TestObject1
    {
      public string Prop1 { get; set; }
      public int Prop2 { get; set; }
    }

    private class TestObject2
    {
      public string PropX { get; set; }
      public TestObject1 PropY { get; set; }
    }

    private class TestObject3
    {
      public TestObject2[] List { get; set; }
    }
  }
}