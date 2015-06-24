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

      actions[0x08] = state => state.Registers[0] = 0;
      actions[0x09] = state => state.Registers[1] = 0;
      actions[0x0A] = state => state.Registers[2] = 0;
      actions[0x0B] = state => state.Registers[3] = 0;
      actions[0x0C] = state => state.Registers[4] = 0;
      actions[0x0D] = state => state.Registers[5] = 0;
      actions[0x0E] = state => state.Registers[6] = 0;
      actions[0x0F] = state => state.Registers[7] = 0;

      actions[0x10] = state => state.Registers[0]++;
      actions[0x11] = state => state.Registers[1]++;
      actions[0x12] = state => state.Registers[2]++;
      actions[0x13] = state => state.Registers[3]++;
      actions[0x14] = state => state.Registers[4]++;
      actions[0x15] = state => state.Registers[5]++;
      actions[0x16] = state => state.Registers[6]++;
      actions[0x17] = state => state.Registers[7]++;
    }

    public void Execute(State state, params byte[] bytes)
    {
      var code = bytes[0];
      if (!actions.ContainsKey(code))
        return;

      actions[code](state);
    }

    //

    private readonly Dictionary<byte, Action<State>> actions;
  }
}