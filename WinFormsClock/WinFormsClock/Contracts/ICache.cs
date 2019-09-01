using System;

namespace WinFormsClock.Contracts
{
    public interface ICache<in TKey, TItem> : IDisposable
        where TItem : IDisposable
    {
        TItem Get(TKey key, Func<TItem> constructor);
    }
}