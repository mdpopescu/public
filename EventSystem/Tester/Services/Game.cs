using EventSystem.Library.Contracts;
using System;
using System.Threading.Tasks;

namespace Tester.Services;

internal class Game
{
    public Game(IEventBus events, IResultsCache results)
    {
        this.events = events;
        this.results = results;
    }

    public async Task<int> RunAsync()
    {
        var id = Guid.NewGuid();
        await events.PublishAsync(new InitCommand(id));
        return await results.GetAsync<int>(id, TIMEOUT);
    }

    //

    private static readonly TimeSpan TIMEOUT = TimeSpan.FromMinutes(5);

    private readonly IEventBus events;
    private readonly IResultsCache results;
}