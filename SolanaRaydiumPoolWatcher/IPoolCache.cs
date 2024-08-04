using Solnet.Wallet;

namespace SolanaRaydiumPoolWatcher
{
    /// <summary>
    /// Just simple state for the pool (For demo purposes)
    /// </summary>
    public struct PoolState
    {
        public ulong PoolOpenTime { get; }
        public PublicKey BaseMint { get; }
        public PublicKey QuoteMint { get; }

        public PoolState(ulong poolOpenTime, PublicKey baseMint, PublicKey quoteMint)
        {
            PoolOpenTime = poolOpenTime;
            BaseMint = baseMint;
            QuoteMint = quoteMint;
        }
    }

    public interface IPoolCache
    {
        Task SaveAsync(PublicKey key, PoolState poolState);
        Task<PoolState?> GetAsync(PublicKey key);
        bool Has(PublicKey key);
        Task RemoveAsync(PublicKey key);
    }
}
