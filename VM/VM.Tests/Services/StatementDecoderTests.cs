using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VM.Library.Models;
using VM.Library.Services;

namespace VM.Tests.Services
{
  [TestClass]
  public class StatementDecoderTests
  {
    private StatementDecoder sut;
    private State state;

    [TestInitialize]
    public void SetUp()
    {
      sut = new StatementDecoder();
      state = new State
      {
        Memory = new byte[65536],
        Registers = new ushort[8],
        StackPointer = 0,
        ProgramCounter = 0,
      };
    }

    [TestMethod]
    public void ClearsTheRegisters()
    {
      ForRegisters(r =>
      {
        state.Registers[r] = 1;

        sut.Execute(state, (byte) (0x08 + r));

        Assert.AreEqual(0, state.Registers[r]);
      });
    }

    [TestMethod]
    public void IncrementsTheRegisters()
    {
      ForRegisters(r =>
      {
        state.Registers[r] = 1;

        sut.Execute(state, (byte) (0x10 + r));

        Assert.AreEqual(2, state.Registers[r]);
      });
    }

    [TestMethod]
    public void DecrementsTheRegisters()
    {
      ForRegisters(r =>
      {
        state.Registers[r] = 3;

        sut.Execute(state, (byte)(0x18 + r));

        Assert.AreEqual(2, state.Registers[r]);
      });
    }

    //

    private static void ForRegisters(Action<byte> action)
    {
      for (byte r = 0; r < 8; r++)
        action(r);
    }
  }
}