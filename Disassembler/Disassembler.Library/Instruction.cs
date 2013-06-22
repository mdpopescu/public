namespace Renfield.Disassembler.Library
{
  public class Instruction
  {
    public LockRepPrefix? LockRepPrefix { get; private set; }
    public SegPrefix? SegPrefix { get; private set; }
    public bool OpSizePrefix { get; private set; }
    public bool AddrSizePrefix { get; private set; }
  }
}