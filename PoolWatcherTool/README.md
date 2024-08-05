# Solana Pool Watcher Tool

The **Solana Pool Watcher Tool** is a tool designed to monitor liquidity pools on the Solana blockchain, specifically focusing on Raydium pools. It utilizes Solnet to interact with the Solana RPC and listen for changes in pool accounts.

## Features
- Monitors liquidity pools on Solana (currently supports Raydium pools).
- Subscribes to new pool events.
- Transaction monitoring.

## Prerequisites

- .NET SDK 6.0 or later (Only tested on .NET 8.0)
- A Solana RPC endpoint (MainNet)

## Example Result

Here's an example of the Solana Raydium Pool Watcher in action (PoolWatcher.Example):
```
info: Program[0]
      New pool detected:
      ID: DkRRPoqCoEMFn8VR7pzEFudS7i9SgYCZN9iNaXKAtTTV
      Base Mint: AFzoAY8yyAMahfXt8vJ766vTxSvN63ebPtcSg6yMRbu8
      Base Lot Size: 1000000
      Trade Fee: 0
      Swap Fee: 0
      
info: Program[0]
      New Pool Detected and Do something: AFzoAY8yyAMahfXt8vJ766vTxSvN63ebPtcSg6yMRbu8
info: Program[0]
      New Transaction: 5KmiqFvUjYya7TgWaAYKmFqDrjiRwYqhZmj6a7cviEfiQrBbXcYTL2eBpDUbHWk47YFVibYKAwJDLZ2YMZuL2BN7
info: Program[0]
      New Transaction: 33KrQ7t3JWGk9z2Ai4mboWfSPqLH9yC2kReopLbvjuuhHjBNCTmESuXUZZG2dXttfSLeZSMuobDrmWUPJZ9F6EYA
```

## Contact

If you want to leave a tip, you can send it to the following address (SOL): 
```
D95T4R4wbaSGUPjKKnnKcs3jnobZXAk4Z5EVrfpWCEcu
```
