using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VM.Library.Contracts;
using VM.Library.Models;
using VM.Library.Services;

namespace VM.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    [Ignore]
    public void AddsTwoNumbersAndWritesTheResult()
    {
      var ram = new byte[65536];
      SetProgram(ram);
      var reg = new ushort[8];
      var state = new State
      {
        Memory = ram,
        Registers = reg,
        StackPointer = 0,
        ProgramCounter = 0,
      };
      var decoder = new StatementDecoder();
      var io = new Mock<LineIO>();
      var sut = new Machine(state, decoder, io.Object);

      sut.Execute();

      io.Verify(it => it.WriteLine("8"));
    }

    //

    private static void SetProgram(IList<byte> ram)
    {
      ram[0x00] = 0x28; // SET r0, 5
      ram[0x01] = 0x05;
      ram[0x02] = 0x00;
      ram[0x03] = 0x29; // SET r1, 3
      ram[0x04] = 0x03;
      ram[0x05] = 0x00;
      ram[0x06] = 0x81; // ADD r1 -- r0 = 8
      ram[0x07] = 0x29; // SET r1, '0'
      ram[0x08] = 0x30;
      ram[0x09] = 0x00;
      ram[0x0A] = 0x81; // ADD r1 -- adds '0' to the value so it can be printed
      ram[0x0B] = 0x38; // STOR r0, [00F0]
      ram[0x0C] = 0xF0;
      ram[0x0D] = 0x00;
      ram[0x0E] = 0x08; // CLR r0
      ram[0x0F] = 0x38; // STOR r0, [00F1] -- terminates the string with a NUL
      ram[0x10] = 0xF1;
      ram[0x11] = 0x00;
      ram[0x12] = 0x87; // PUTS [00F0] -- writes the string to the console
      ram[0x13] = 0xF0;
      ram[0x14] = 0x00;
      ram[0x15] = 0xFF; // HALT
    }
  }
}