using System;
using VM.Library.Contracts;
using VM.Library.Models;

namespace VM.Library.Services
{
  public class StatementDecoder : Decoder
  {
    public StatementDecoder()
    {
      actions = new Action<State>[256];

      SetUpDecodingTable();
    }

    public void Execute(State state)
    {
      var code = state.GetByte();
      var action = actions[code];
      if (action != null)
        action(state);
    }

    //

    private readonly Action<State>[] actions;

    private void SetUpDecodingTable()
    {
      for (byte r = 0; r < 8; r++)
      {
        var rr = r;

        actions[(byte) (0x08 + rr)] = state => state.Registers[rr] = 0;
        actions[(byte) (0x10 + rr)] = state => state.Registers[rr]++;
        actions[(byte) (0x18 + rr)] = state => state.Registers[rr]--;
        actions[(byte) (0x20 + rr)] = state => state.Registers[rr] = (ushort) ~state.Registers[rr];
        actions[(byte) (0x28 + rr)] = state => state.Registers[rr] = state.GetWord();
        actions[(byte) (0x30 + rr)] = state => state.Registers[rr] = state.LoadWord(state.GetWord());
        actions[(byte) (0x38 + rr)] = state => state.SaveWord(state.GetWord(), state.Registers[rr]);
        actions[(byte) (0x40 + rr)] = state => state.Registers[0] += state.Registers[rr];
        actions[(byte) (0x48 + rr)] = state => state.Registers[0] -= state.Registers[rr];
        actions[(byte) (0x50 + rr)] = state => state.Registers[0] &= state.Registers[rr];
        actions[(byte) (0x58 + rr)] = state => state.Registers[0] |= state.Registers[rr];
        actions[(byte) (0x60 + rr)] = state => state.Registers[rr] >>= 1;
        actions[(byte) (0x68 + rr)] = state => state.Registers[rr] <<= 1;
        actions[(byte) (0x70 + rr)] = state => state.Registers[rr] = (ushort) ((state.Registers[rr] >> 1) | (state.Registers[rr] << 15));
        actions[(byte) (0x78 + rr)] = state => state.Registers[rr] = (ushort) ((state.Registers[rr] << 1) | (state.Registers[rr] >> 15));
      }

      actions[0x80] = state => state.ProgramCounter = state.GetWord();
      actions[0x81] = state =>
      {
        var addr = state.GetWord();
        if (state.Registers[0] == 0)
          state.ProgramCounter = addr;
      };
      actions[0x82] = state =>
      {
        var addr = state.GetWord();
        if (state.Registers[0] != 0)
          state.ProgramCounter = addr;
      };
      actions[0x84] = state =>
      {
        var addr = state.GetWord();

        // save the return address (the current program counter) on the stack -- high byte first because the stack grows downwards
        state.Memory[--state.StackPointer] = (byte) (state.ProgramCounter >> 8);
        state.Memory[--state.StackPointer] = (byte) (state.ProgramCounter & 0xFF);

        // jump to the subroutine
        state.ProgramCounter = addr;
      };
    }
  }
}