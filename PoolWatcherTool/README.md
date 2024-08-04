# Solana Pool Watcher Tool

The **Solana Raydium Pool Watcher** is a tool designed to monitor liquidity pools on the Solana blockchain, specifically focusing on Raydium pools. It utilizes Solnet to interact with the Solana RPC and listen for changes in pool accounts.

## Features
- Monitors liquidity pools on Solana (currently supports Raydium pools).
- Subscribes to new pool events.
- Future plans include adding transaction monitoring features.

## Prerequisites

- .NET SDK 6.0 or later (Only tested on .NET 8.0)
- A Solana RPC endpoint (MainNet)

## Example Results

### Solana Pool Watcher Tool

Here's an example of the Solana Raydium Pool Watcher in action:
```
info: SolanaRaydiumPoolWatcher.Program[0]
      New pool detected:
ID: CmiBYysEsmJT7KfwrzxoXZFYoHwnMEeG6RJ8mWcWg62a
Base Mint: EdUHX6GU7ecC6zsEcPitpPjnK8HgCAjY85AuL5ovURqd
Base Lot Size: 1000000000
Trade Fee: 0
Swap Fee: 0

info: SolanaRaydiumPoolWatcher.Program[0]
      New pool detected:
ID: 4m3TPrhMvXQPFRd8pywq1tdyCXZCTVYb7iTRv1YvrQso
Base Mint: 3NTgo8kfWSJ2NbpjHKWtATy9oMMTcsSijUrWqdrLGeTW
Base Lot Size: 100000000
Trade Fee: 0
```

## Contact

If you want to leave a tip, you can send it to the following address (SOL): 
```
D95T4R4wbaSGUPjKKnnKcs3jnobZXAk4Z5EVrfpWCEcu
```
