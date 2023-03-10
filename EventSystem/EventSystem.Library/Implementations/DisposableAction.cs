using System;

namespace EventSystem.Library.Implementations;

public class DisposableAction : IDisposable
{
    public DisposableAction(Action action)
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