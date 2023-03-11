namespace CustomDialogs.Contracts;

public interface IInterceptor
{
    nint IdHook { get; set; }

    void SetHandler(Action<nint, nint> handler);
    int HookMethod(int nCode, nint wParam, nint lParam);
}