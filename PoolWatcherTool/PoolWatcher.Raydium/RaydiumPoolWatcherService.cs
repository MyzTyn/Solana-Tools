using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using PoolWatcher.Raydium.Config;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;

namespace PoolWatcher.Raydium;

/// <summary>
/// The Pool Watcher Service for Raydium.
/// </summary>
public class RaydiumPoolWatcherService : IPoolWatcherService
{
    private ILogger? _logger;
    private Task<SubscriptionState> _onProgramAccountChange;
    /// <summary>
    /// Key: Pool ID
    /// Value: List of transactions (ToDo: Implement the transaction model)
    /// </summary>
    private Dictionary<PublicKey, List<LiquidityStateV4>> _pools;
    
    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public event EventHandler<object>? OnNewPoolDetected;
    public event EventHandler<object>? OnTransactionDetected;

    public IEnumerable<object> GetPools()
    {
       throw new NotImplementedException();
    }

    public IEnumerable<object> GetTransactions()
    {
        throw new NotImplementedException();
    }

    public Task Subscribe(IStreamingRpcClient streamingRpcClient, ILogger? logger = null)
    {
        _pools = new Dictionary<PublicKey, List<LiquidityStateV4>>();
        _logger = logger;
        
        // Define the filters (By the warp-id/solana-trading-bot)
        int quoteMintOffset = (int)Marshal.OffsetOf<RawLiquidityStateV4>("QuoteMint");
        int marketProgramIdOffset = (int)Marshal.OffsetOf<RawLiquidityStateV4>("MarketProgramId");
        int statusOffset = (int)Marshal.OffsetOf<RawLiquidityStateV4>("Status");
        string statusBytes = Encoders.Base58.EncodeData([6, 0, 0, 0, 0, 0, 0, 0]);
        int dataSize = Marshal.SizeOf<RawLiquidityStateV4>();
        
        // Log the filter setup
        _logger?.LogInformation($"Filter Setup:\n" +
                                $"QuoteMint Offset: {quoteMintOffset}\n" +
                                $"Bytes: {Constants.WSOL.Mint}\n\n" +
                                $"MarketProgramId Offset: {marketProgramIdOffset}\n" +
                                $"Bytes: {Constants.MAINNET_PROGRAM_ID.OPENBOOK_MARKET}\n\n" +
                                $"Status Offset: {statusOffset}\n" +
                                $"Bytes: {statusBytes}\n\n" +
                                $"Data Size: {dataSize}"
        );
        
        var filters = new List<MemCmp>
        {
            new MemCmp()
            {
                Offset = quoteMintOffset,
                Bytes = Constants.WSOL.Mint
            },
            new MemCmp()
            {
                Offset = marketProgramIdOffset,
                Bytes = Constants.MAINNET_PROGRAM_ID.OPENBOOK_MARKET
            },
            new MemCmp()
            {
                Offset = statusOffset,
                Bytes = statusBytes
            }
        };
        
        // Subscribe to account changes
        _onProgramAccountChange = streamingRpcClient.SubscribeProgramAsync(
            Constants.MAINNET_PROGRAM_ID.AmmV4,
            (SubscriptionState state, ResponseValue<AccountKeyPair> data) =>
            {
                // Get the time with 113 offset (sightly delayed)
                long runTimestamp = (long)Math.Floor(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0) - 113;
                
                // Decode the raw liquidity state
                LiquidityStateV4 poolState = new LiquidityStateV4(RawLiquidityStateV4.Decode(Convert.FromBase64String(data.Value.Account.Data[0])));
                
                if (!_pools.ContainsKey(poolState.BaseMint) && (long)poolState.PoolOpenTime > runTimestamp)
                {
                    // Log the new pool
                    logger?.LogInformation($"New pool detected:\n" +
                                           $"ID: {data.Value.PublicKey}\n" +
                                           $"Base Mint: {poolState.BaseMint}\n" +
                                           $"Base Lot Size: {poolState.BaseLotSize}\n" +
                                           $"Trade Fee: {poolState.TradeFee}\n" +
                                           $"Swap Fee: {poolState.SwapFee}\n");
                    
                    // Create a new pool
                    _pools.Add(poolState.BaseMint, new List<LiquidityStateV4> { poolState });
                    // Trigger the event
                    OnNewPoolDetected?.Invoke(this, poolState);
                    
                    // ToDo: Subscribe to the pool transactions
                }
            },
            Commitment.Confirmed,
            Marshal.SizeOf<RawLiquidityStateV4>(),
            filters
        );
        
        return Task.CompletedTask;
    }

    public Task Unsubscribe(IStreamingRpcClient streamingRpcClient)
    {
        if (_onProgramAccountChange == null)
            throw new InvalidOperationException("Not subscribed to pool updates.");
        
        return streamingRpcClient.UnsubscribeAsync(_onProgramAccountChange.Result);
    }
}