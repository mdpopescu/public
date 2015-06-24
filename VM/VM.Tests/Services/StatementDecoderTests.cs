using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VM.Library.Contracts;
using VM.Library.Models;
using VM.Library.Services;

namespace VM.Tests.Services
{
  [TestClass]
  public class StatementDecoderTests
  {
    private Mock<LineIO> io;
    private StatementDecoder sut;
    private State state;

    [TestInitialize]
    public void SetUp()
    {
      io = new Mock<LineIO>();
      sut = new StatementDecoder(io.Object);
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

    [TestMethod]
    public void AddsTheRegisterToTheAccumulator()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x40 + r));
        state.Registers[0] = 5;
        state.Registers[r] = 5;
      }, r => Assert.AreEqual(10, state.Registers[0]));
    }

    [TestMethod]
    public void SubtractsTheRegisterFromTheAccumulator()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x48 + r));
        state.Registers[0] = 5;
        state.Registers[r] = 5;
      }, r => Assert.AreEqual(0, state.Registers[0]));
    }

    [TestMethod]
    public void AndsTheRegisterToTheAccumulator()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x50 + r));
        state.Registers[0] = 5;
        state.Registers[r] = 2;
      }, r => Assert.AreEqual(r == 0 ? 2 : 0, state.Registers[0]));
    }

    [TestMethod]
    public void OrsTheRegisterToTheAccumulator()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x58 + r));
        state.Registers[0] = 5;
        state.Registers[r] = 2;
      }, r => Assert.AreEqual(r == 0 ? 2 : 7, state.Registers[0]));
    }

    [TestMethod]
    public void ShiftsTheRegisterToTheRight()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x60 + r));
        state.Registers[r] = 5;
      }, 2);
    }

    [TestMethod]
    public void ShiftsTheRegisterToTheRightWhenTheLeftmostBitIsSet()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x60 + r));
        state.Registers[r] = 0xFF00;
      }, 0x7F80);
    }

    [TestMethod]
    public void ShiftsTheRegisterToTheLeft()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x68 + r));
        state.Registers[r] = 5;
      }, 10);
    }

    [TestMethod]
    public void ShiftsTheRegisterToTheLeftWhenTheRightmostBitIsSet()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x68 + r));
        state.Registers[r] = 0xFF;
      }, 0x01FE);
    }

    [TestMethod]
    public void RotatesTheRegisterToTheRight()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x70 + r));
        state.Registers[r] = 4;
      }, 2);
    }

    [TestMethod]
    public void RotatesTheRegisterToTheRightWhenTheRightmostBitIsSet()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x70 + r));
        state.Registers[r] = 0x00FF;
      }, 0x807F);
    }

    [TestMethod]
    public void RotatesTheRegisterToTheLeft()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x78 + r));
        state.Registers[r] = 5;
      }, 10);
    }

    [TestMethod]
    public void RotatesTheRegisterToTheLeftWhenTheLeftmostBitIsSet()
    {
      ForRegisters(r =>
      {
        state.AddByte((byte) (0x78 + r));
        state.Registers[r] = 0xFF00;
      }, 0xFE01);
    }

    [TestMethod]
    public void JumpsToTheGivenAddress()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x80);
      state.AddByte(0x11);
      state.AddByte(0x11);

      state.ProgramCounter = 0;
      sut.Execute(state);

      Assert.AreEqual(0x1111, state.ProgramCounter);
    }

    [TestMethod]
    public void JumpsWhenAccumulatorIsZero()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x81);
      state.AddByte(0x11);
      state.AddByte(0x11);
      state.Registers[0] = 0;

      state.ProgramCounter = 0;
      sut.Execute(state);

      Assert.AreEqual(0x1111, state.ProgramCounter);
    }

    [TestMethod]
    public void DoesNotJumpWhenAccumulatorIsNotZero()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x81);
      state.AddByte(0x11);
      state.AddByte(0x11);
      state.Registers[0] = 1;

      state.ProgramCounter = 0;
      sut.Execute(state);

      Assert.AreEqual(3, state.ProgramCounter);
    }

    [TestMethod]
    public void JumpsWhenAccumulatorIsNotZero()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x82);
      state.AddByte(0x11);
      state.AddByte(0x11);
      state.Registers[0] = 1;

      state.ProgramCounter = 0;
      sut.Execute(state);

      Assert.AreEqual(0x1111, state.ProgramCounter);
    }

    [TestMethod]
    public void DoesNotJumpWhenAccumulatorIsZero()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x82);
      state.AddByte(0x11);
      state.AddByte(0x11);
      state.Registers[0] = 0;

      state.ProgramCounter = 0;
      sut.Execute(state);

      Assert.AreEqual(3, state.ProgramCounter);
    }

    [TestMethod]
    public void CallsSubroutine()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x84);
      state.AddByte(0x11);
      state.AddByte(0x11);

      state.ProgramCounter = 0;
      sut.Execute(state);

      // verify that the stack contains the return address 0x0003
      Assert.AreEqual(0x00, state.Memory[0xFFFF]);
      Assert.AreEqual(0x03, state.Memory[0xFFFE]);
      // verify that the stack pointer points to the top of the stack
      Assert.AreEqual(0xFFFE, state.StackPointer);
      // verify that the program counter has changed
      Assert.AreEqual(0x1111, state.ProgramCounter);
    }

    [TestMethod]
    public void ReturnsFromSubroutine()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x85);
      state.Push(0x1112);

      state.ProgramCounter = 0;
      sut.Execute(state);

      Assert.AreEqual(0x1112, state.ProgramCounter);
    }

    [TestMethod]
    public void ReadsStringAndStoresItInMemory()
    {
      state.ProgramCounter = 0;
      state.AddByte(0x86);
      state.AddWord(0x1111);
      io
        .Setup(it => it.ReadLine())
        .Returns("ABC");

      state.ProgramCounter = 0;
      sut.Execute(state);

      Assert.AreEqual(0x41, state.Memory[0x1111]);
      Assert.AreEqual(0x42, state.Memory[0x1112]);
      Assert.AreEqual(0x43, state.Memory[0x1113]);
      Assert.AreEqual(0x00, state.Memory[0x1114]);
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