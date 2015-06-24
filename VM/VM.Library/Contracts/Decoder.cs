using VM.Library.Models;

namespace VM.Library.Contracts
{
  public interface Decoder
  {
    byte Execute(State state);
  }
}