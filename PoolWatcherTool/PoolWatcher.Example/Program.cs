// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PoolWatcher;
using PoolWatcher.Raydium;
using PoolWatcher.Raydium.Config;

// Dependency Injection
ServiceProvider services = new ServiceCollection().AddLogging(configure => configure.AddConsole())
    .BuildServiceProvider();

// Get the logger
ILogger<Program>? logger = services.GetService<ILoggerFactory>()?.CreateLogger<Program>();

// Set up the Streaming RPC client without logging
Solnet.Rpc.IStreamingRpcClient _streamingRpcClient = Solnet.Rpc.ClientFactory.GetStreamingClient(Solnet.Rpc.Cluster.MainNet);

// Set up the IPoolWatcherService
IPoolWatcherService poolWatcherService = new RaydiumPoolWatcherService();

// Set up the new pool detected event
poolWatcherService.OnNewPoolDetected += (sender, poolState) =>
{
    if (poolState is LiquidityStateV4 liquidityState)
    {
        logger?.LogInformation($"New Pool Detected and Do something: {liquidityState.BaseMint}");
        // Do something with the pool ...
    }
};

// Set up the new transaction detected event
poolWatcherService.OnTransactionDetected += (sender, transaction) =>
{
    if (transaction is LiquidityStateV4 liquidityState)
    {
        logger?.LogInformation($"New Transaction Detected and Do something:\n Trade Fee:{liquidityState.TradeFee}");
        // Do something with the transaction ...
    }
};

// Connect to the Solana Main
await _streamingRpcClient.ConnectAsync();

// Subscribe to the pool
await poolWatcherService.Subscribe(_streamingRpcClient, logger);

// Wait for the user to exit
logger?.LogInformation("Subscribed to program account changes. Press any key to exit...");
Console.ReadKey();

// Unsubscribe from the pool
await poolWatcherService.Unsubscribe(_streamingRpcClient);
await _streamingRpcClient.DisconnectAsync();
