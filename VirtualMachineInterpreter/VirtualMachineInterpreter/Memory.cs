using System;
using System.Linq;

namespace Renfield.VM
{
  public class Memory
  {
    public Memory(int size)
    {
      memory = new byte[size];
      ip = 0;

      Array.Clear(memory, 0, size);
    }

    public void Set(byte[] bytes)
    {
      if (bytes.Length > memory.Length)
        throw new Exception("Buffer overflow - trying to load more than the memory size.");

      Array.Copy(bytes, memory, bytes.Length);
    }

    public void Jump(int address)
    {
      if (address >= memory.Length)
        throw new Exception(string.Format("Memory access violation - invalid address {0:X8}", address));

      ip = address;
    }

    public byte GetByte()
    {
      return GetBytes(1)[0];
    }

    public ushort GetUShort()
    {
      return BitConverter.ToUInt16(GetBytes(2), 0);
    }

    public int GetInteger()
    {
      return BitConverter.ToInt32(GetBytes(4), 0);
    }

    //

    private readonly byte[] memory;

    private int ip;

    private byte[] GetBytes(int count)
    {
      var result = memory.Skip(ip).Take(count).ToArray();
      ip += count;

      return result;
    }
  }
}