using System.Collections.Concurrent;
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
    /// Value: The latest pool data
    /// </summary>
    private ConcurrentDictionary<PublicKey, LiquidityStateV4> _pools;
    /// <summary>
    /// Key: Pool ID
    /// Value
    /// </summary>
    private ConcurrentDictionary<PublicKey, List<TransactionInfo>> _transactions;
    /// <summary>
    /// Signal for Transaction Monitoring
    /// </summary>
    private CancellationTokenSource cancellationTokenSource;
    
    public void Initialize()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Sender: RaydiumPoolWatcherService
    /// Object: LiquidityStateV4
    /// </summary>
    public event EventHandler<object>? OnNewPoolDetected;
    /// <summary>
    /// Sender: The Pool's BaseMint
    /// Object: TransactionInfo
    /// </summary>
    public event EventHandler<object>? OnTransactionDetected;

    public IEnumerable<object> GetPools()
    {
        return _pools.Values.Cast<object>();
    }

    public IEnumerable<object> GetTransactions(PublicKey poolID)
    {
        return _transactions[poolID];
    }

    public Task Subscribe(IStreamingRpcClient streamingRpcClient, ILogger? logger = null)
    {
        _pools = new ConcurrentDictionary<PublicKey, LiquidityStateV4>();
        _transactions = new ConcurrentDictionary<PublicKey, List<TransactionInfo>>();
        cancellationTokenSource = new CancellationTokenSource();
        
        _logger = logger;
        
        IRpcClient client = Solnet.Rpc.ClientFactory.GetClient(Cluster.MainNet);
        
        // Define the filters (By the warp-id/solana-trading-bot)
        int quoteMintOffset = (int)Marshal.OffsetOf<RawLiquidityStateV4>("QuoteMint");
        int marketProgramIdOffset = (int)Marshal.OffsetOf<RawLiquidityStateV4>("MarketProgramId");
        int statusOffset = (int)Marshal.OffsetOf<RawLiquidityStateV4>("Status");
        string statusBytes = Encoders.Base58.EncodeData([6, 0, 0, 0, 0, 0, 0, 0]);
        int dataSize = Marshal.SizeOf<RawLiquidityStateV4>();
        
        // Log the filters setup
        _logger?.LogInformation($"Filter Setup:\n" +
                                $"QuoteMint Offset: {quoteMintOffset}\n" +
                                $"Bytes: {Constants.WSOL.Mint}\n\n" +
                                $"MarketProgramId Offset: {marketProgramIdOffset}\n" +
                                $"Bytes: {Constants.MAINNET_PROGRAM_ID.OPENBOOK_MARKET}\n\n" +
                                $"Status Offset: {statusOffset}\n" +
                                $"Bytes: {statusBytes}\n\n" +
                                $"Data Size: {dataSize}"
        );
        
        // Filters setup
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
                
                // Update the Pool
                if (_pools.ContainsKey(poolState.BaseMint))
                {
                    _pools[poolState.BaseMint] = poolState;
                }
                
                // The new pool
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
                    _pools[poolState.BaseMint] = poolState;
                    // Trigger the event
                    OnNewPoolDetected?.Invoke(this, poolState);
                }
            },
            Commitment.Confirmed,
            Marshal.SizeOf<RawLiquidityStateV4>(),
            filters
        );
        
        // Start the Transaction Monitoring
        MonitoringTransaction(client, cancellationTokenSource.Token);
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Ineffective method of Transaction Monitoring by 5 seconds delay (Websocket is better method but use third party)
    /// </summary>
    /// <param name="client"></param>
    private async Task MonitoringTransaction(IRpcClient client, CancellationToken cancellationToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(new TimeSpan(0, 0, 0, 5, 0));
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            foreach (var key in _pools.Keys)
            {
                if (!_transactions.ContainsKey(key))
                {
                    _transactions[key] = new List<TransactionInfo>();
                }

                var result = await client.GetSignaturesForAddressAsync(key, 1, commitment: Commitment.Confirmed);

                if (!result.WasSuccessful)
                    continue;

                var transaction = await client.GetTransactionAsync(result.Result[0].Signature);
                
                if (!transaction.WasSuccessful)
                    continue;
                
                if (_transactions[key].Any(t => t.Signatures == transaction.Result.Transaction.Signatures))
                    continue;
                
                _transactions[key].Add(transaction.Result.Transaction);
                OnTransactionDetected?.Invoke(key, transaction.Result.Transaction);
            }
        }
    }
    
    public Task Unsubscribe(IStreamingRpcClient streamingRpcClient)
    {
        if (_onProgramAccountChange == null)
            throw new InvalidOperationException("Not subscribed to pool updates.");
        
        cancellationTokenSource.Cancel();
        
        return streamingRpcClient.UnsubscribeAsync(_onProgramAccountChange.Result);
    }
}