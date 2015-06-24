﻿using System;
using System.Text;
using VM.Library.Contracts;
using VM.Library.Models;
using Decoder = VM.Library.Contracts.Decoder;

namespace VM.Library.Services
{
  public class StatementDecoder : Decoder
  {
    public StatementDecoder(LineIO io)
    {
      this.io = io;

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

    private readonly LineIO io;

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
        // read the address *first*, otherwise the value saved on the stack will be incorrect
        var addr = state.GetWord();

        state.Push(state.ProgramCounter);
        state.ProgramCounter = addr;
      };
      actions[0x85] = state => state.ProgramCounter = state.Pop();
      actions[0x86] = state =>
      {
        var addr = state.GetWord();

        var line = io.ReadLine();
        var bytes = Encoding.UTF8.GetBytes(line);

        foreach (var b in bytes)
        {
          state.Memory[addr++] = b;
        }

        // add the final NUL character
        state.Memory[addr] = 0;
      };
    }
  }
}