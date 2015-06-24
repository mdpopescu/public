namespace VM.Library.Models
{
  public class State
  {
    public byte[] Memory { get; set; }
    public ushort[] Registers { get; set; }
    public ushort StackPointer { get; set; }
    public ushort ProgramCounter { get; set; }

    public byte GetByte()
    {
      return Memory[ProgramCounter++];
    }

    public ushort GetWord()
    {
      return (ushort) (GetByte() + (GetByte() << 8));
    }

    public void AddByte(byte b)
    {
      Memory[ProgramCounter++] = b;
    }

    public ushort LoadWord()
    {
      var addr = GetWord();
      return (ushort) (Memory[addr] + (Memory[addr + 1] << 8));
    }
  }
}