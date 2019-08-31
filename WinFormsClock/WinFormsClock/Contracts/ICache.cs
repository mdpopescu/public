using System;

namespace WinFormsClock.Contracts
{
    public interface ICache<TItem, in TKey> : IDisposable
        where TItem : IDisposable
    {
        TItem Get(TKey key, Func<TItem> constructor);
    }
}