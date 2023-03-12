﻿using CustomDialogs.Contracts;

namespace CustomDialogs.Library_64.Services;

public class Interceptor : IInterceptor
{
    public nint IdHook { get; set; } = nint.Zero;

    // ReSharper disable once ParameterHidesMember
    //public void SetHandler(Action<nint, nint> handler)
    //{
    //    this.handler = handler;
    //}

    public int HookMethod(int nCode, nint wParam, nint lParam)
    {
        //Console.WriteLine($"nCode={nCode}, wParam={wParam}, lParam={lParam}");

        //if (nCode >= 0 && (long)wParam > 0)
        //{
        //    var msg = (Native.MSG)Marshal.PtrToStructure(lParam, typeof(Native.MSG))!;
        //    Console.WriteLine($"message={msg.message}");
        //}

        //return 0;

        using (var stream = new FileStream(@"c:\temp\custom.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
        using (var writer = new StreamWriter(stream))
        {
            writer.WriteLine($"nCode={nCode}, wParam={wParam}, lParam={lParam}");
            writer.Flush();
        }

        //Console.WriteLine($"nCode={nCode}, wParam={wParam}, lParam={lParam}");

        //if (nCode >= Native.HC_ACTION)
        //    handler?.Invoke(wParam, lParam);

        return Native.CallNextHookEx(IdHook, nCode, wParam, lParam);
    }

    //

    private Action<nint, nint>? handler;
}