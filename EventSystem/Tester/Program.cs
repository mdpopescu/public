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

        var game = new Game();
        var score = await game.RunAsync();

        Console.SetCursorPosition(0, 20);
        Console.WriteLine($"Final score: {score}");

        Console.CursorVisible = true;
        Console.ReadLine();
    }
}