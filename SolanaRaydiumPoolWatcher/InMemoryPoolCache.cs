using Solnet.Wallet;
using System.Collections.Concurrent;
namespace SolanaRaydiumPoolWatcher
{
    public class InMemoryPoolCache : IPoolCache
    {
        private readonly ConcurrentDictionary<PublicKey, PoolState> _cache = new ();

        public Task SaveAsync(PublicKey key, PoolState poolState)
        {
            _cache[key] = poolState;
            return Task.CompletedTask;
        }

        public Task<PoolState?> GetAsync(PublicKey key)
        {
            PoolState? poolState = _cache[key];
            return Task.FromResult(poolState);
        }

        public Task RemoveAsync(PublicKey key)
        {
            _cache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public bool Has(PublicKey key)
        {
            return _cache.ContainsKey(key);
        }
    }

}
