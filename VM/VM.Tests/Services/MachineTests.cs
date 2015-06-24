using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VM.Library.Contracts;
using VM.Library.Models;
using VM.Library.Services;

namespace VM.Tests.Services
{
  [TestClass]
  public class MachineTests
  {
    private State state;
    private Mock<Decoder> decoder;
    private Mock<LineIO> io;
    private Machine sut;

    [TestInitialize]
    public void SetUp()
    {
      state = new State
      {
        Memory = new byte[65536],
        Registers = new ushort[8],
        StackPointer = 0,
        ProgramCounter = 0,
      };
      decoder = new Mock<Decoder>();
      io = new Mock<LineIO>();
      sut = new Machine(state, decoder.Object);
    }

    [TestMethod]
    public void CallsTheDecoderToInterpretTheInstructions()
    {
      state.AddByte(0x00); // NOP
      state.AddByte(0xFF); // HALT

      state.ProgramCounter = 0;
      sut.Execute();

      decoder.Verify(it => it.Execute(state));
    }

    [TestMethod]
    public void OnlyDecodesInstructionsUntilHalt()
    {
      state.AddByte(0x00); // NOP
      state.AddByte(0x00); // NOP
      state.AddByte(0x00); // NOP
      state.AddByte(0xFF); // HALT

      state.ProgramCounter = 0;
      sut.Execute();

      decoder.Verify(it => it.Execute(state), Times.Exactly(3));
    }

    [TestMethod]
    public void StartsDecodingFromTheCurrentProgramCounter()
    {
      state.AddByte(0x00); // NOP
      state.AddByte(0x00); // NOP
      state.AddByte(0x00); // NOP
      state.AddByte(0xFF); // HALT

      state.ProgramCounter = 2;
      sut.Execute();

      decoder.Verify(it => it.Execute(state), Times.Exactly(1));
    }

    [TestMethod]
    public void UpdatesTheProgramCounterToTheAddressFollowingTheHaltStatement()
    {
      state.AddByte(0x00); // NOP
      state.AddByte(0x00); // NOP
      state.AddByte(0x00); // NOP
      state.AddByte(0xFF); // HALT

      state.ProgramCounter = 0;
      sut.Execute();

      Assert.AreEqual(4, state.ProgramCounter);
    }
  }
}