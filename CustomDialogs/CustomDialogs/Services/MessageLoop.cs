using System.Diagnostics.CodeAnalysis;

namespace CustomDialogs.Services;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MessageLoop
{
    private Action InitialAction { get; }
    private Thread? Thread { get; set; }

    public bool IsRunning { get; private set; }

    public MessageLoop(Action initialAction)
    {
        InitialAction = initialAction;
    }

    public void Start()
    {
        IsRunning = true;

        Thread = new Thread(
            () =>
            {
                InitialAction.Invoke();

                while (IsRunning)
                {
                    var result = Native.GetMessage(out var message, nint.Zero, 0, 0);
                    if (result <= 0)
                    {
                        Stop();

                        continue;
                    }

                    Native.TranslateMessage(ref message);
                    Native.DispatchMessage(ref message);
                }
            });

        Thread.Start();
    }

    public void Stop()
    {
        IsRunning = false;
    }
}