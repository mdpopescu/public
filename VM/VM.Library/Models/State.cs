namespace VM.Library.Models
{
  public class State
  {
    public byte[] Memory { get; set; }
    public ushort[] Registers { get; set; }
    public ushort StackPointer { get; set; }
    public ushort ProgramCounter { get; set; }
  }
}