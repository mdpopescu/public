using VM.Library.Contracts;
using VM.Library.Models;

namespace VM.Library.Services
{
  public class Machine
  {
    public Machine(State state, Decoder decoder, LineIO io)
    {
      this.state = state;
      this.decoder = decoder;
      this.io = io;
    }

    public void Execute()
    {
      while (true)
      {
        var b = state.Memory[state.ProgramCounter++];
        if (b == 0xFF)
          return;

        decoder.Execute(state);
      }
    }

    //

    private readonly State state;
    private readonly Decoder decoder;
    private readonly LineIO io;
  }
}