using System;
using System.Collections.Generic;

namespace Renfield.VM
{
  public class Interpreter
  {
    public Interpreter()
    {
      stack = new Stack<int>(STACK_SIZE);
      memory = new Memory(MEM_SIZE);
      registers = new Registers(REG_COUNT);

      instructions = new Action[256];
      RegisterInstructions();
    }

    public void Run(byte[] bytes)
    {
      if (bytes.Length > MEM_SIZE)
        throw new Exception(string.Format("Program too large - {0} bytes greater than memory size of {1} bytes.", bytes.Length, MEM_SIZE));

      memory.Set(bytes);

      stack.Clear();
      memory.Jump(0);

      do
      {
        var instruction = memory.GetByte();
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
    private const int MEM_SIZE = 65536;
    private const int REG_COUNT = 16;

    private readonly Stack<int> stack;
    private readonly Memory memory;
    private readonly Registers registers;

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

    private void DoNop()
    {
      //
    }

    private void DoPush()
    {
      var arg = memory.GetInteger();
      stack.Push(arg);
    }

    private void DoPop()
    {
      stack.Pop();
    }

    private void DoLoad()
    {
      var arg = memory.GetByte();
      stack.Push(registers[arg]);
    }

    private void DoStore()
    {
      var arg = memory.GetByte();
      registers[arg] = stack.Pop();
    }

    private void DoJmp()
    {
      var arg = memory.GetUShort();
      memory.Jump(arg);
    }

    private void DoJz()
    {
      var tos = stack.Pop();
      if (tos == 0)
      {
        var address = memory.GetUShort();
        memory.Jump(address);
      }
    }

    private void DoJnz()
    {
      var tos = stack.Pop();
      if (tos != 0)
      {
        var address = memory.GetUShort();
        memory.Jump(address);
      }
    }

    private void DoAdd()
    {
      var s1 = stack.Pop();
      var s2 = stack.Pop();
      stack.Push(s2 + s1);
    }

    private void DoSub()
    {
      var s1 = stack.Pop();
      var s2 = stack.Pop();
      stack.Push(s2 - s1);
    }

    private void DoMul()
    {
      var s1 = stack.Pop();
      var s2 = stack.Pop();
      stack.Push(s2 * s1);
    }

    private void DoDiv()
    {
      var s1 = stack.Pop();
      var s2 = stack.Pop();
      stack.Push(s2 / s1);
    }

    private void DoPrint()
    {
      var tos = stack.Pop();
      Console.WriteLine(tos);
    }

    private void DoStop()
    {
      throw new Exception("Program halted.");
    }
  }
}