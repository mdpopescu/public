using EventSystem.Library.Contracts;
using System;
using System.Threading.Tasks;
using Tester.Models;

namespace Tester.Services;

internal class Game
{
    public Game(IEventBus events, IResultsCache results)
    {
        this.events = events;
        this.results = results;

        events.Subscribe<InitCommand>(InitHandlerAsync);
        events.Subscribe<MoveLeftCommand>(MoveLeftHandlerAsync);
        events.Subscribe<MoveRightCommand>(MoveRightHandlerAsync);
        events.Subscribe<DrawCommand>(DrawHandlerAsync);
    }

    public async Task<int> RunAsync()
    {
        var id = Guid.NewGuid();

        await events.PublishAsync(new InitCommand());

        return await results.GetAsync<int>(id, TIMEOUT);
    }

    //

    private const int MAX_OFFSET = 9;

    private const int BOARD_X = 30;
    private const int BOARD_Y = 10;

    private const char EMPTY = '-';
    private const char PLAYER = 'o';

    private static readonly TimeSpan TIMEOUT = TimeSpan.FromMinutes(5);

    private readonly IEventBus events;
    private readonly IResultsCache results;

    private int score;
    private int position;

    private async Task InitHandlerAsync(InitCommand msg)
    {
        score = 0;
        position = 0;

        await events.PublishAsync(new DrawCommand());
    }

    private async Task MoveLeftHandlerAsync(MoveLeftCommand msg)
    {
        if (position > -MAX_OFFSET)
        {
            position--;
            await events.PublishAsync(new DrawCommand());
        }
    }

    private async Task MoveRightHandlerAsync(MoveRightCommand msg)
    {
        if (position < MAX_OFFSET)
        {
            position++;
            await events.PublishAsync(new DrawCommand());
        }
    }

    private Task DrawHandlerAsync(DrawCommand msg)
    {
        // draw the board
        Console.SetCursorPosition(BOARD_X, BOARD_Y);
        Console.Write(new string(EMPTY, MAX_OFFSET * 2 + 1));

        // draw the player
        var playerX = BOARD_X + MAX_OFFSET + position;
        Console.SetCursorPosition(playerX, BOARD_Y);
        Console.Write(PLAYER);

        // draw the prize
        // todo

        return Task.CompletedTask;
    }
}