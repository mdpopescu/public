using VM.Library.Contracts;

namespace VM.Library.Services
{
  public class Machine
  {
    public Machine(LineIO io, byte[] ram, ushort[] reg)
    {
      this.io = io;
      this.ram = ram;
      this.reg = reg;
    }

    public void Execute(uint start)
    {
      //
    }

    //

    private LineIO io;
    private byte[] ram;
    private ushort[] reg;
  }
}