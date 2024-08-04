# Solana Tools

Solana Tools is a collection of tools and utilities (for now, only one tool published) for interacting with the Solana blockchain.

The **Solana Raydium Pool Watcher** is a tool designed to monitor liquidity pools on the Solana blockchain, specifically focusing on Raydium pools. It utilizes Solnet to interact with the Solana RPC and listen for changes in pool accounts.

## Features
### Solana Raydium Pool Watcher
- Connects to Solana MainNet via streaming RPC client.
- Sets up filters to monitor specific liquidity pools based on mint and program IDs.
- Subscribes to account changes and logs information about new pools.

## Prerequisites

- .NET SDK 6.0 or later (Only tested on .NET 8.0)
- A Solana RPC endpoint (MainNet)

## Setup

1. **Clone the repository:**

    ```bash
    git clone https://github.com/MyzTyn/SolanaTools.git
    cd SolanaTools
    ```

2. **Build the project:**

Open the SolanaTools.sln and build the project.

## Example Results

### Solana Raydium Pool Watcher

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
