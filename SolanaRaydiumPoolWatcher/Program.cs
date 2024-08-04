using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System.Runtime.InteropServices;
using Solnet.Wallet.Utilities;

namespace SolanaRaydiumPoolWatcher
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // Dependency Injection
            ServiceProvider services = new ServiceCollection().
                AddLogging(configure => configure.AddConsole())
                .BuildServiceProvider();

            // Get the logger
            ILogger<Program>? logger = services.GetService<ILoggerFactory>()?.CreateLogger<Program>();
            // ILogger<IStreamingRpcClient>? streamingLogger = services.GetService<ILoggerFactory>()?.CreateLogger<IStreamingRpcClient>();

            // RPC Client
            
            // Solnet.Rpc.IRpcClient client = Solnet.Rpc.ClientFactory.GetClient(Solnet.Rpc.Cluster.MainNet, logger);
            //Solnet.Rpc.IStreamingRpcClient _streamingRpcClient = Solnet.Rpc.ClientFactory.GetStreamingClient(Solnet.Rpc.Cluster.MainNet, streamingLogger);
            
            // Disable the logger
            Solnet.Rpc.IStreamingRpcClient _streamingRpcClient = Solnet.Rpc.ClientFactory.GetStreamingClient(Solnet.Rpc.Cluster.MainNet);
            
            logger?.LogInformation($"Filter Setup:\n" +
                $"QuoteMint Offset: {(int)Marshal.OffsetOf(typeof(RawLiquidityStateV4), "QuoteMint")}\n" +
                $"Bytes: {RaydiumPool.WSOL.Mint}\n\n" +

                $"MarketProgramId Offset: {(int)Marshal.OffsetOf(typeof(RawLiquidityStateV4), "MarketProgramId")}\n" +
                $"Bytes: {RaydiumPool.MAINNET_PROGRAM_ID.OPENBOOK_MARKET}\n\n" +

                $"Status Offset: {(int)Marshal.OffsetOf(typeof(RawLiquidityStateV4), "Status")}\n" +
                $"Bytes: {Encoders.Base58.EncodeData([6, 0, 0, 0, 0, 0, 0, 0])}\n\n" +
                $"Data Size: {Marshal.SizeOf<RawLiquidityStateV4>()}"
                );
            
            // Define the filters (By the RaydiumPool Pool)
            var filters = new List<MemCmp>
            {
                new MemCmp()
                {
                    Offset = (int)Marshal.OffsetOf(typeof(RawLiquidityStateV4), "QuoteMint"),
                    Bytes = RaydiumPool.WSOL.Mint
                },
                new MemCmp()
                {
                    Offset = (int)Marshal.OffsetOf(typeof(RawLiquidityStateV4), "MarketProgramId"),
                    Bytes = RaydiumPool.MAINNET_PROGRAM_ID.OPENBOOK_MARKET
                },
                new MemCmp()
                {
                    Offset = (int)Marshal.OffsetOf(typeof(RawLiquidityStateV4), "Status"),
                    Bytes = Encoders.Base58.EncodeData([6, 0, 0, 0, 0, 0, 0, 0])
                }
            };
            
            // Create a Cache (To Avoid the same pool being added multiple times for Demo purposes)
            IPoolCache poolCache = new InMemoryPoolCache();

            // Connect to the streaming RPC client
            await _streamingRpcClient.ConnectAsync();

            // Subscribe to account changes
            Task<SubscriptionState> onProgramAccountChange = _streamingRpcClient.SubscribeProgramAsync(
                RaydiumPool.MAINNET_PROGRAM_ID.AmmV4,
                (SubscriptionState state, ResponseValue<AccountKeyPair> data) =>
                {
                    // With 113 offset
                    long runTimestamp = (long)Math.Floor(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0) - 113;
                                        
                    LiquidityStateV4 poolState = new LiquidityStateV4();
                    poolState.Raw = RawLiquidityStateV4.Decode(Convert.FromBase64String(data.Value.Account.Data[0]));
                    
                    if (!poolCache.Has(poolState.BaseMint) && (long)poolState.PoolOpenTime > runTimestamp)
                    {
                        poolCache.SaveAsync(poolState.BaseMint, new PoolState(poolState.PoolOpenTime, poolState.BaseMint, poolState.QuoteMint));
                        logger?.LogInformation($"New pool detected:\n" +
                                               $"ID: {data.Value.PublicKey}\n" +
                                               $"Base Mint: {poolState.BaseMint}\n" +
                                               $"Base Lot Size: {poolState.BaseLotSize}\n" +
                                               $"Trade Fee: {poolState.TradeFee}\n" +
                                               $"Swap Fee: {poolState.SwapFee}\n");
                    }
                },
                Commitment.Confirmed,
                Marshal.SizeOf<RawLiquidityStateV4>(),
                filters
            );

            logger?.LogInformation("Subscribed to program account changes. Press any key to exit...");
            Console.ReadKey();

            // Unsubscribe before exiting
            await _streamingRpcClient.DisconnectAsync();
        }
    }
}
