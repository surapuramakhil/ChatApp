using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caching;

public class InMemoryLRUCache<TKey, TValue> : IAsyncCache<TKey, TValue>
{
    private readonly LRUCache<TKey, TValue> _cache;

    public InMemoryLRUCache(int capacity, int evictionCount)
    {
        _cache = new LRUCache<TKey, TValue>(capacity, evictionCount);
    }

    public Task<TValue> GetAsync(TKey key)
    {
        if (_cache.TryGet(key, out var value))
        {
            return Task.FromResult(value);
        }
        return Task.FromResult(default(TValue));
    }

    public Task SetAsync(TKey key, TValue value)
    {
        _cache.AddReplace(key, value);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(TKey key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _cache.Clear();
        return Task.CompletedTask;
    }
}