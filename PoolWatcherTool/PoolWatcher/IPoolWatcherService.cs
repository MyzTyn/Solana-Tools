using Microsoft.Extensions.Logging;
using Solnet.Rpc;
using Solnet.Wallet;

namespace PoolWatcher;

public interface IPoolWatcherService
{
    /// <summary>
    /// Initialize the service.
    /// </summary>
    /// <returns></returns>
    void Initialize();
    
    /// <summary>
    /// The event that is triggered when a new pool is detected.
    /// </summary>
    event EventHandler<object> OnNewPoolDetected;
    
    /// <summary>
    /// The event that is triggered when a new transaction is detected for the pools.
    /// </summary>
    event EventHandler<object> OnTransactionDetected;
    
    /// <summary>
    /// Get the pools.
    /// </summary>
    /// <returns></returns>
    IEnumerable<object> GetPools();
    
    /// <summary>
    /// Get the transactions by Pool ID.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<object> GetTransactions(PublicKey poolID);
    
    /// <summary>
    /// Subscribe to the Pool Watcher Service.
    /// </summary>
    /// <param name="streamingRpcClient"></param>
    /// <returns></returns>
    Task Subscribe(IStreamingRpcClient streamingRpcClient, ILogger? logger = null);
    
    /// <summary>
    /// Unsubscribe from pool updates.
    /// </summary>
    /// <returns></returns>
    Task Unsubscribe(IStreamingRpcClient streamingRpcClient);
}