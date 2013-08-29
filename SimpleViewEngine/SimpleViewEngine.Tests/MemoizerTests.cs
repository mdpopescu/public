using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SimpleViewEngine.Library.Caching;

namespace Renfield.SimpleViewEngine.Tests
{
  public class MemoizerTests
  {
    [TestClass]
    public class ZeroArgs
    {
      [TestMethod]
      public void CallsTheUnderlyingFuncOnFirstCall()
      {
        var called = false;
        Func<int> original = () =>
        {
          called = true;
          return 5;
        };
        var memoized = original.Memoize();

        var result = memoized.Invoke();

        Assert.AreEqual(5, result);
        Assert.IsTrue(called);
      }

      [TestMethod]
      public void ReturnsCachedValueForSubsequentCalls()
      {
        var called = 0;
        Func<int> original = () =>
        {
          called++;
          return 5;
        };
        var memoized = original.Memoize();

        memoized.Invoke();
        memoized.Invoke();
        var result = memoized.Invoke();

        Assert.AreEqual(5, result);
        Assert.AreEqual(1, called);
      }
    }

    [TestClass]
    public class OneArg
    {
      [TestMethod]
      public void CallsTheUnderlyingFuncOnFirstCall()
      {
        var called = false;
        Func<int, int> original = arg =>
        {
          called = true;
          return arg * 5;
        };
        var memoized = original.Memoize();

        var result = memoized.Invoke(7);

        Assert.AreEqual(35, result);
        Assert.IsTrue(called);
      }

      [TestMethod]
      public void ReturnsCachedValueForSubsequentCalls()
      {
        var called = 0;
        Func<int, int> original = arg =>
        {
          called++;
          return arg * 5;
        };
        var memoized = original.Memoize();

        memoized.Invoke(7);
        memoized.Invoke(7);
        var result = memoized.Invoke(7);

        Assert.AreEqual(35, result);
        Assert.AreEqual(1, called);
      }

      [TestMethod]
      public void CallsTheUnderlyingFuncWhenCalledWithDifferentArguments()
      {
        var called = new Dictionary<int, bool>();
        Func<int, int> original = arg =>
        {
          called[arg] = true;

          return arg * 5;
        };
        var memoized = original.Memoize();

        var result1 = memoized.Invoke(7);
        var result2 = memoized.Invoke(8);

        Assert.AreEqual(35, result1);
        Assert.AreEqual(40, result2);
        Assert.IsTrue(called[7]);
        Assert.IsTrue(called[8]);
      }
    }

    [TestClass]
    public class TwoArgs
    {
      [TestMethod]
      public void CallsTheUnderlyingFuncOnFirstCall()
      {
        var called = false;
        Func<int, int, int> original = (a1, a2) =>
        {
          called = true;
          return a1 * a2 * 5;
        };
        var memoized = original.Memoize();

        var result = memoized.Invoke(7, 8);

        Assert.AreEqual(280, result);
        Assert.IsTrue(called);
      }

      [TestMethod]
      public void ReturnsCachedValueForSubsequentCalls()
      {
        var called = 0;
        Func<int, int, int> original = (a1, a2) =>
        {
          called++;
          return a1 * a2 * 5;
        };
        var memoized = original.Memoize();

        memoized.Invoke(7, 8);
        memoized.Invoke(7, 8);
        var result = memoized.Invoke(7, 8);

        Assert.AreEqual(280, result);
        Assert.AreEqual(1, called);
      }

      [TestMethod]
      public void CallsTheUnderlyingFuncWhenCalledWithDifferentArguments()
      {
        var called = new Dictionary<int, bool>();
        Func<int, int, int> original = (a1, a2) =>
        {
          called[a1 * 10 + a2] = true;

          return a1 * a2 * 5;
        };
        var memoized = original.Memoize();

        var result1 = memoized.Invoke(7, 8);
        var result2 = memoized.Invoke(8, 7);

        Assert.AreEqual(280, result1);
        Assert.AreEqual(280, result2);
        Assert.IsTrue(called[78]);
        Assert.IsTrue(called[87]);
      }
    }
  }
}