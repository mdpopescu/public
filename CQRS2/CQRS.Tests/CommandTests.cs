﻿using System;
using System.Linq;
using System.Threading;
using CQRS.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.Tests
{
  [TestClass]
  public class CommandTests
  {
    [TestMethod]
    public void CallsMethodOfTargetObject()
    {
      var obj = new MyClass1();

      obj.SendCommand("M1", 5);

      Thread.Sleep(10);
      Assert.AreEqual(5, obj.X);
    }

    [TestMethod]
    public void CallsMethodMissingIfMethodIsNotFound()
    {
      var obj = new MyClass1();

      obj.SendCommand("M2", 3);

      Thread.Sleep(10);
      Assert.AreEqual("M2(3)", obj.MissingMethodCall);
    }

    [TestMethod]
    [ExpectedException(typeof (MissingMethodException))]
    public void ThrowsAnExceptionIfMethodMissingCannotBeFound()
    {
      var obj = new object();

      obj.SendCommand("M1");
    }

    //

    private class MyClass1
    {
      public int X { get; private set; }
      public string MissingMethodCall { get; private set; }

      public void MethodMissing(string name, params object[] args)
      {
        MissingMethodCall = name + "(" + string.Join(",", args.Select(it => it.ToString())) + ")";
        Thread.Sleep(2000);
      }

      public void M1(int x)
      {
        X = x;
      }

      public void M3()
      {
        Thread.Sleep(2000);
      }
    }

    private class MyClass2
    {
      public void MethodMissing(string name, params object[] args)
      {
        throw new Exception("test2");
      }

      public void M1()
      {
        throw new Exception("test");
      }
    }
  }
}