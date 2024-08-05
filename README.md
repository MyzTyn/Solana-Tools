# Solana Tools

Solana Tools is a collection of tools and utilities (for now, only one tool published) for interacting with the Solana blockchain.

The **Solana Pool Watcher** is a tool designed to monitor liquidity pools on the Solana blockchain, specifically focusing on Raydium pools. It utilizes Solnet to interact with the Solana RPC and listen for changes in pool accounts.

## Tools
### Solana Pool Watcher Tool
- Connects to Solana MainNet via streaming RPC client.
- Sets up filters to monitor specific liquidity pools based on mint and program IDs.
- Subscribes to account changes and transactions information about new pools.

## Upcoming Tools
Additional tools are planned and will be added in the future. Stay tuned for updates!

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

## Contact

If you want to leave a tip, you can send it to the following address (SOL): 
```
D95T4R4wbaSGUPjKKnnKcs3jnobZXAk4Z5EVrfpWCEcu
```
