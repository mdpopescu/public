using EventSystem.Library.Implementations;
using System;
using System.Threading.Tasks;
using Tester.Services;

namespace Tester;

internal class Program
{
    private static async Task Main()
    {
        Console.Clear();
        Console.CursorVisible = false;

        var events = new EventBus();

        var kbd = new KeyboardMapper(events);
        using (kbd.SetUp())
        {
            var game = new Game(
                events,
                new ResultsCache(
                    new TimeProvider(),
                    events
                )
            );
            var score = await game.RunAsync();

            Console.SetCursorPosition(0, 20);
            Console.WriteLine($"Final score: {score}");
        }

        Console.CursorVisible = true;
        Console.ReadLine();
    }
}