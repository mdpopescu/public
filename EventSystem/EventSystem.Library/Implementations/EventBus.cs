using EventSystem.Library.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSystem.Library.Implementations;

public class EventBus : IEventBus
{
    public Task PublishAsync(IMessage msg)
    {
        var list = GetList(msg.GetType());
        foreach (var handler in list)
            handler(msg); // this invokes the handler without waiting for it to complete
        return Task.CompletedTask;
    }

    public IDisposable Subscribe<T>(Func<T, Task> handler)
        where T : IMessage
    {
        var list = GetList(typeof(T));

        // we can't have a list of Func<T, Task> so we need to convert the handler to be able to add it to the list
        Task NewHandler(object o) => handler((T)o);
        list.Add(NewHandler);
        return new DisposableAction(() => list.Remove(NewHandler));
    }

    //

    private readonly Dictionary<Type, List<Func<object, Task>>> handlers = new();

    private List<Func<object, Task>> GetList(Type key)
    {
        if (!handlers.ContainsKey(key))
            handlers.Add(key, new List<Func<object, Task>>());
        return handlers[key];
    }
}