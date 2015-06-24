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
        state.AddByte((byte) (0x08 + r));
        state.Registers[r] = 1;
      }, 0);
    }

    [TestMethod]
    public void IncrementsTheRegisters()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x10 + r));
        state.Registers[r] = 1;
      }, 2);
    }

    [TestMethod]
    public void DecrementsTheRegisters()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x18 + r));
        state.Registers[r] = 3;
      }, 2);
    }

    [TestMethod]
    public void NegatesTheRegisters()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x20 + r));
        state.Registers[r] = 0x5555;
      }, 0xAAAA);
    }

    [TestMethod]
    public void SetsTheRegisterToTheGivenValue()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x28 + r));
        state.AddByte(0xFF);
        state.AddByte(0x00);
      }, 255);
    }

    [TestMethod]
    public void LoadsTheRegisterFromAnAddress()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x30 + r));
        state.AddByte(0xF0);
        state.AddByte(0x00);
        state.Memory[0x00F0] = 0x12;
        state.Memory[0x00F1] = 0x34;
      }, 0x3412);
    }

    [TestMethod]
    public void SavesTheRegisterToAnAddress()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x38 + r));
        state.AddByte(0xF0);
        state.AddByte(0x00);
        state.Registers[r] = (ushort) (0x1200 + r);
      }, r =>
      {
        Assert.AreEqual(r, state.Memory[0x00F0]);
        Assert.AreEqual(0x12, state.Memory[0x00F1]);
      });
    }

    //

    private void ForRegisters(Action<byte> action, ushort expected)
    {
      for (byte r = 0; r < 8; r++)
      {
        state.ProgramCounter = 0;
        action(r);

        state.ProgramCounter = 0;
        sut.Execute(state);

        Assert.AreEqual(expected, state.Registers[r]);
      }
    }

    private void ForRegisters(Action<byte> action, Action<byte> check)
    {
      for (byte r = 0; r < 8; r++)
      {
        state.ProgramCounter = 0;
        action(r);

        state.ProgramCounter = 0;
        sut.Execute(state);

        check(r);
      }
    }
  }
}