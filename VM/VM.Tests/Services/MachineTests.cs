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
      sut = new Machine(state, decoder.Object, io.Object);
    }

    [TestMethod]
    public void CallsTheDecoderToInterpretTheInstructions()
    {
      state.Memory[0] = 0x00; // NOP
      state.Memory[1] = 0xFF; // HALT

      sut.Execute();

      decoder.Verify(it => it.Execute(state, 0x00));
    }

    [TestMethod]
    public void OnlyDecodesInstructionsUntilHalt()
    {
      state.Memory[0] = 0x00; // NOP
      state.Memory[1] = 0x00; // NOP
      state.Memory[2] = 0x00; // NOP
      state.Memory[3] = 0xFF; // HALT

      sut.Execute();

      decoder.Verify(it => it.Execute(state, It.IsAny<byte[]>()), Times.Exactly(3));
    }

    [TestMethod]
    public void StartsDecodingFromTheCurrentProgramCounter()
    {
      state.Memory[0] = 0x00; // NOP
      state.Memory[1] = 0x01; // NOP
      state.Memory[2] = 0x02; // NOP
      state.Memory[3] = 0xFF; // HALT
      state.ProgramCounter = 2;

      sut.Execute();

      decoder.Verify(it => it.Execute(state, It.IsAny<byte[]>()), Times.Exactly(1));
    }

    [TestMethod]
    public void UpdatesTheProgramCounterToTheAddressFollowingTheHaltStatement()
    {
      state.Memory[0] = 0x00; // NOP
      state.Memory[1] = 0x00; // NOP
      state.Memory[2] = 0x00; // NOP
      state.Memory[3] = 0xFF; // HALT

      sut.Execute();

      Assert.AreEqual(4, state.ProgramCounter);
    }
  }
}