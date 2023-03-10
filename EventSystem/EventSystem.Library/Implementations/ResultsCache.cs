using EventSystem.Library.Contracts;
using EventSystem.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSystem.Library.Implementations;

public class ResultsCache : IResultsCache
{
    public ResultsCache(ITimeProvider timeProvider, IEventBus events)
    {
        this.timeProvider = timeProvider;

        events.Subscribe<SetResultMessage>(SetResultHandlerAsync);
    }

    public async Task<T?> GetAsync<T>(Guid id, TimeSpan timeout)
    {
        var start = timeProvider.Now;
        while (true)
        {
            if (results.ContainsKey(id))
                return (T?)results[id];

            await Task.Delay(WAIT_INTERVAL);
            if (timeProvider.Now - start > timeout)
                return default;
        }
    }

    //

    private const int WAIT_INTERVAL = 250;

    // this will have to be a time-limited object cache, but for now we'll use a regular dictionary
    private readonly Dictionary<Guid, object> results = new();

    private readonly ITimeProvider timeProvider;

    private Task SetResultHandlerAsync(SetResultMessage msg)
    {
        results[msg.Id] = msg.Result;
        return Task.CompletedTask;
    }
}