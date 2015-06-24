using VM.Library.Contracts;
using VM.Library.Models;

namespace VM.Library.Services
{
  public class Machine
  {
    public Machine(State state, Decoder decoder)
    {
      this.state = state;
      this.decoder = decoder;
    }

    public void Execute()
    {
      byte code;
      do
      {
        code = decoder.Execute(state);
      } while (code != 0xFF);
    }

    //

    private readonly State state;
    private readonly Decoder decoder;
  }
}