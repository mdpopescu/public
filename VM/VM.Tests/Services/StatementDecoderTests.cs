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
        state.Memory[0] = (byte) (0x08 + r);
        state.Registers[r] = 1;
        state.ProgramCounter = 0;

        sut.Execute(state);

        Assert.AreEqual(0, state.Registers[r]);
      });
    }

    [TestMethod]
    public void IncrementsTheRegisters()
    {
      ForRegisters(r =>
      {
        state.Memory[0] = (byte) (0x10 + r);
        state.Registers[r] = 1;
        state.ProgramCounter = 0;

        sut.Execute(state);

        Assert.AreEqual(2, state.Registers[r]);
      });
    }

    [TestMethod]
    public void DecrementsTheRegisters()
    {
      ForRegisters(r =>
      {
        state.Memory[0] = (byte) (0x18 + r);
        state.Registers[r] = 3;
        state.ProgramCounter = 0;

        sut.Execute(state);

        Assert.AreEqual(2, state.Registers[r]);
      });
    }

    [TestMethod]
    public void NegatesTheRegisters()
    {
      ForRegisters(r =>
      {
        state.Memory[0] = (byte) (0x20 + r);
        state.Registers[r] = 0x5555;
        state.ProgramCounter = 0;

        sut.Execute(state);

        Assert.AreEqual(0xAAAA, state.Registers[r]);
      });
    }

    [TestMethod]
    public void SetsTheRegisterToTheGivenValue()
    {
      ForRegisters(r =>
      {
        state.Memory[0] = (byte) (0x28 + r);
        state.Memory[1] = 0xFF;
        state.Memory[2] = 0x00;
        state.ProgramCounter = 0;

        sut.Execute(state);

        Assert.AreEqual(255, state.Registers[r]);
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