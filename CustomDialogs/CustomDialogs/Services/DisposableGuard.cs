namespace CustomDialogs.Services;

public class DisposableGuard : IDisposable
{
    public DisposableGuard(Action action)
    {
        this.action = action;
    }

    public void Dispose()
    {
        action();
    }

    //

    private readonly Action action;
}