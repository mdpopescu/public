using System;

namespace Renfield.VM
{
  public class Interpreter
  {
    public Interpreter()
    {
      stack = new int[STACK_SIZE];
      registers = new int[REG_COUNT];
      memory = new byte[MEM_SIZE];

      instructions = new Action[256];
      RegisterInstructions();
    }

    public void Run(byte[] bytes)
    {
      if (bytes.Length > MEM_SIZE)
        throw new Exception(string.Format("Program too large - {0} bytes greater than memory size of {1} bytes.", bytes.Length, MEM_SIZE));

      Array.Copy(bytes, memory, bytes.Length);

      sp = 0;
      ip = 0;

      do
      {
        var instruction = GetByte();
        var action = instructions[instruction];

        try
        {
          action();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          break;
        }
      } while (true);
    }

    //

    private const int STACK_SIZE = 256;
    private const int REG_COUNT = 16;
    private const byte REG_MASK = REG_COUNT - 1;
    private const int MEM_SIZE = 65536;

    private readonly int[] stack;
    private byte sp;
    private readonly int[] registers;
    private readonly byte[] memory;
    private ushort ip;

    private readonly Action[] instructions;

    private void RegisterInstructions()
    {
      for (var i = 0; i < instructions.Length; i++)
        instructions[i] = DoNop;

      instructions[0x01] = DoPush;
      instructions[0x02] = DoPop;
      instructions[0x03] = DoLoad;
      instructions[0x04] = DoStore;
      instructions[0x05] = DoJmp;
      instructions[0x06] = DoJz;
      instructions[0x07] = DoJnz;
      instructions[0x08] = DoAdd;
      instructions[0x09] = DoSub;
      instructions[0x0A] = DoMul;
      instructions[0x0B] = DoDiv;
      instructions[0x0C] = DoPrint;
      instructions[0x0D] = DoStop;
    }

    private byte GetByte()
    {
      return memory[ip++];
    }

    private ushort GetUShort()
    {
      unchecked
      {
        return (ushort) (GetByte() + 256 * GetByte());
      }
    }

    private int GetInteger()
    {
      unchecked
      {
        return GetUShort() + 65536 * GetUShort();
      }
    }

    private void Push(int n)
    {
      stack[sp++] = n;
    }

    private int Pop()
    {
      return stack[--sp];
    }

    private void DoNop()
    {
      //
    }

    private void DoPush()
    {
      var arg = GetInteger();
      Push(arg);
    }

    private void DoPop()
    {
      Pop();
    }

    private void DoLoad()
    {
      var arg = GetByte();
      var r = arg & REG_MASK;
      Push(registers[r]);
    }

    private void DoStore()
    {
      var arg = GetByte();
      var r = arg & REG_MASK;
      registers[r] = Pop();
    }

    private void DoJmp()
    {
      var arg = GetUShort();
      ip = arg;
    }

    private void DoJz()
    {
      var tos = Pop();
      if (tos == 0)
        ip = GetUShort();
    }

    private void DoJnz()
    {
      var tos = Pop();
      if (tos != 0)
        ip = GetUShort();
    }

    private void DoAdd()
    {
      var s1 = Pop();
      var s2 = Pop();
      Push(s2 + s1);
    }

    private void DoSub()
    {
      var s1 = Pop();
      var s2 = Pop();
      Push(s2 - s1);
    }

    private void DoMul()
    {
      var s1 = Pop();
      var s2 = Pop();
      Push(s2 * s1);
    }

    private void DoDiv()
    {
      var s1 = Pop();
      var s2 = Pop();
      Push(s2 / s1);
    }

    private void DoPrint()
    {
      var tos = Pop();
      Console.WriteLine(tos);
    }

    private void DoStop()
    {
      throw new Exception("Program halted.");
    }
  }
}