using System;
using System.Runtime.InteropServices;

namespace FindDuplicates.Services
{
  internal static class NativeMethods
  {
    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);
  }
}