﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransformyClone.Library;

namespace TransformyClone.Tests
{
  [TestClass]
  public class ListTransformerTests
  {
    private ListTransformer sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new ListTransformer();
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnNullInputList()
    {
      sut.Transform(null, "x");
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnEmptyInputList()
    {
      sut.Transform(new List<string>(), "x");
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnNullSample()
    {
      sut.Transform(new List<string> { "a" }, null);
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnEmptySample()
    {
      sut.Transform(new List<string> { "a" }, "");
    }
  }
}