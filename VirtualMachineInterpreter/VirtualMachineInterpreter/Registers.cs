using System;

namespace Renfield.VM
{
  public class Registers
  {
    public Registers(byte count)
    {
      if ((count & (count - 1)) != 0)
        throw new Exception("Invalid number of registers (must be a power of two).");

      registers = new int[count];
      mask = (byte) (count - 1);
    }

    public int this[byte r]
    {
      get { return registers[r & mask]; }
      set { registers[r & mask] = value; }
    }

    //

    private readonly int[] registers;
    private readonly byte mask;
  }
}