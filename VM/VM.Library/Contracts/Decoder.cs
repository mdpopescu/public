using VM.Library.Models;

namespace VM.Library.Contracts
{
  public interface Decoder
  {
    void Execute(State state);
  }
}