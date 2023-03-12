using CustomDialogs.Contracts;
using System.Diagnostics;
using System.Reflection;

namespace CustomDialogs.Services;

public class HookManager
{
    public IDisposable InstallHook(Process targetProcess)
    {
        var width = Native.Is64Bit(targetProcess) ? "64" : "32";
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Cannot determine the DLL folder.");
        var filename = Path.Combine(path, $"CustomDialogs.Library_{width}.dll");
        var assembly = Assembly.LoadFrom(filename);
        var typeName = $"CustomDialogs.Library_{width}.Services.Interceptor";
        var instance = DynamicFactory.Create(assembly, typeName);

        if (instance is not IInterceptor interceptor)
            throw new Exception("Cannot create the Interceptor.");

        //setUp(interceptor);

        var dllHandle = Native.GetModuleHandle(filename);
        var threadId = Native.GetWindowThreadProcessId(targetProcess.MainWindowHandle, nint.Zero);
        var idHook = Native.SetWindowsHookEx(Native.HookType.WH_CALLWNDPROC, interceptor.HookMethod, dllHandle/*nint.Zero*/, threadId);
        interceptor.IdHook = idHook;

        return new DisposableGuard(() => Native.UnhookWindowsHookEx(idHook));
    }
}