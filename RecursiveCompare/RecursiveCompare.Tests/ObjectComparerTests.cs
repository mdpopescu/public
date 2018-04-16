using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.RecursiveCompare.Tests
{
    [TestClass]
    public class ObjectComparerTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ThrowsIfObjectsDoNotHaveSameType()
        {
            var obj1 = new TestObject1();
            var obj2 = new TestObject2();
            var sut = new ObjectComparer();

            sut.Compare(obj1, obj2);
        }

        [TestMethod]
        public void ComparesObjectValuesIfObjectsHaveNoProperties()
        {
            var obj1 = new TestObject1();
            var obj2 = new TestObject1();
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(1, result.Count);
            CheckComparison(result[0], ComparisonResult.NotEqual, "", obj1, obj2);
        }

        [TestMethod]
        public void ReturnsEqualForIntProperty()
        {
            const int VALUE = 123;

            var obj1 = new TestObject2 { Prop1 = VALUE };
            var obj2 = new TestObject2 { Prop1 = VALUE };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(1, result.Count);
            CheckComparison(result[0], ComparisonResult.Equal, ".Prop1", VALUE, VALUE);
        }

        [TestMethod]
        public void ReturnsNotEqualForIntProperty()
        {
            const int VALUE1 = 123;
            const int VALUE2 = 456;

            var obj1 = new TestObject2 { Prop1 = VALUE1 };
            var obj2 = new TestObject2 { Prop1 = VALUE2 };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(1, result.Count);
            CheckComparison(result[0], ComparisonResult.NotEqual, ".Prop1", VALUE1, VALUE2);
        }

        [TestMethod]
        public void ReturnsEqualForStringProperty()
        {
            const string VALUE = "abc";

            var obj1 = new TestObject3 { Prop1 = VALUE };
            var obj2 = new TestObject3 { Prop1 = VALUE };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(1, result.Count);
            CheckComparison(result[0], ComparisonResult.Equal, ".Prop1", VALUE, VALUE);
        }

        [TestMethod]
        public void ReturnsNotEqualForStringProperty()
        {
            const string VALUE1 = "abc";
            const string VALUE2 = "def";

            var obj1 = new TestObject3 { Prop1 = VALUE1 };
            var obj2 = new TestObject3 { Prop1 = VALUE2 };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(1, result.Count);
            CheckComparison(result[0], ComparisonResult.NotEqual, ".Prop1", VALUE1, VALUE2);
        }

        [TestMethod]
        public void ReturnsEqualForEnumProperty()
        {
            const Enum4 VALUE = Enum4.Something;

            var obj1 = new TestObject4 { Prop1 = VALUE };
            var obj2 = new TestObject4 { Prop1 = VALUE };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(1, result.Count);
            CheckComparison(result[0], ComparisonResult.Equal, ".Prop1", VALUE, VALUE);
        }

        [TestMethod]
        public void ReturnsNotEqualForEnumProperty()
        {
            const Enum4 VALUE1 = Enum4.Something;
            const Enum4 VALUE2 = Enum4.SomethingElse;

            var obj1 = new TestObject4 { Prop1 = VALUE1 };
            var obj2 = new TestObject4 { Prop1 = VALUE2 };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(1, result.Count);
            CheckComparison(result[0], ComparisonResult.NotEqual, ".Prop1", VALUE1, VALUE2);
        }

        [TestMethod]
        public void ComparesEachItemInArrayProperty()
        {
            var obj1 = new TestObject5 { PropX = new[] { new TestObject2 { Prop1 = 123 }, new TestObject2 { Prop1 = 456 }, } };
            var obj2 = new TestObject5 { PropX = new[] { new TestObject2 { Prop1 = 789 }, new TestObject2 { Prop1 = 456 }, } };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(3, result.Count);
            CheckComparison(result[0], ComparisonResult.Equal, ".PropX.Length", 2, 2);
            CheckComparison(result[1], ComparisonResult.NotEqual, ".PropX[0].Prop1", 123, 789);
            CheckComparison(result[2], ComparisonResult.Equal, ".PropX[1].Prop1", 456, 456);
        }

        [TestMethod]
        public void IndicesPastArrayEndAreTreatedAsNull()
        {
            var obj1 = new TestObject5 { PropX = new[] { new TestObject2 { Prop1 = 123 }, } };
            var obj2 = new TestObject5 { PropX = new[] { new TestObject2 { Prop1 = 789 }, new TestObject2 { Prop1 = 456 }, } };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, obj2).ToList();

            Assert.AreEqual(3, result.Count);
            CheckComparison(result[0], ComparisonResult.NotEqual, ".PropX.Length", 1, 2);
            CheckComparison(result[1], ComparisonResult.NotEqual, ".PropX[0].Prop1", 123, 789);
            CheckComparison(result[2], ComparisonResult.NotEqual, ".PropX[1].Prop1", null, 456);
        }

        [TestMethod]
        public void IfOneObjectIsNullTheOtherIsStillExploredRecursively()
        {
            var obj1 = new TestObject5 { PropX = new[] { new TestObject2 { Prop1 = 123 }, new TestObject2 { Prop1 = 456 }, } };
            var sut = new ObjectComparer();

            var result = sut.Compare(obj1, null).ToList();

            Assert.AreEqual(3, result.Count);
            CheckComparison(result[0], ComparisonResult.NotEqual, ".PropX.Length", 2, 0);
            CheckComparison(result[1], ComparisonResult.NotEqual, ".PropX[0].Prop1", 123, null);
            CheckComparison(result[2], ComparisonResult.NotEqual, ".PropX[1].Prop1", 456, null);
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
        }

        private class TestObject2
        {
            public int Prop1 { get; set; }
        }

        private class TestObject3
        {
            public string Prop1 { get; set; }
        }

        private enum Enum4
        {
            Something,
            SomethingElse,
        }

        private class TestObject4
        {
            public Enum4 Prop1 { get; set; }
        }

        private class TestObject5
        {
            public TestObject2[] PropX { get; set; }
        }
    }
}