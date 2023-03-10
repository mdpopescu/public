using EventSystem.Library.Contracts;
using EventSystem.Library.Implementations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tester.Models;

namespace Tester.Services;

internal class KeyboardMapper
{
    public KeyboardMapper(EventBus events)
    {
        this.events = events;
    }

    public IDisposable SetUp()
    {
        var cts = new CancellationTokenSource();
        var task = Task.Run(() => HandleKeysAsync(cts.Token), cts.Token);

        async void StopHandlingKeys()
        {
            cts.Cancel();
            await task;
        }

        return new DisposableAction(StopHandlingKeys);
    }

    //

    private static readonly TimeSpan WAIT_INTERVAL = TimeSpan.FromMilliseconds(100);

    private readonly EventBus events;

    private async Task HandleKeysAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var keys = Console.ReadKey(true);
                var cmd = MapKeyToCommand(keys.Key);
                await events.PublishAsync(cmd);

                // clear the console key buffer
                while (Console.KeyAvailable)
                    Console.ReadKey(true);
            }

            await Task.Delay(WAIT_INTERVAL, token);
        }
    }

    private static IMessage MapKeyToCommand(ConsoleKey key) => key switch
    {
        ConsoleKey.LeftArrow => new MoveLeftCommand(),
        ConsoleKey.RightArrow => new MoveRightCommand(),
        _ => new NullCommand(),
    };
}