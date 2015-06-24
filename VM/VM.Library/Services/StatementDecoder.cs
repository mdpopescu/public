using System;
using System.Collections.Generic;
using VM.Library.Contracts;
using VM.Library.Models;

namespace VM.Library.Services
{
  public class StatementDecoder : Decoder
  {
    public StatementDecoder()
    {
      actions = new Dictionary<byte, Action<State>>();

      SetUpDecodingTable();
    }

    public void Execute(State state)
    {
      var code = state.GetByte();
      if (actions.ContainsKey(code))
        actions[code](state);
    }

    //

    private readonly Dictionary<byte, Action<State>> actions;

    private void SetUpDecodingTable()
    {
      for (byte r = 0; r < 8; r++)
      {
        var rr = r;

        actions[(byte) (0x00 + r)] = _ => { };
        actions[(byte) (0x08 + r)] = state => state.Registers[rr] = 0;
        actions[(byte) (0x10 + r)] = state => state.Registers[rr]++;
        actions[(byte) (0x18 + r)] = state => state.Registers[rr]--;
        actions[(byte) (0x20 + r)] = state => state.Registers[rr] = (ushort) ~state.Registers[rr];
        actions[(byte) (0x28 + r)] = state => state.Registers[rr] = state.GetWord();
      }
    }
  }
}