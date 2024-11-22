using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAsyncCache<TKey, TValue>
{
    Task<TValue> GetAsync(TKey key);
    Task SetAsync(TKey key, TValue value);
    Task RemoveAsync(TKey key);
    Task ClearAsync();
}