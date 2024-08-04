using Solnet.Wallet;

namespace PoolWatcher.Raydium.Config;

/// <summary>
/// Raydium SDK Token
/// </summary>
public struct Token
{
    public readonly PublicKey ProgramID;
    public readonly PublicKey Mint;
    public readonly decimal Decimals;
    public readonly string Symbol;
    public readonly string Name;
    

    public Token(PublicKey programID, PublicKey mint, decimal decimals, string symbol, string name)
    {
        ProgramID = programID;
        Mint = mint;
        Decimals = decimals;
        Symbol = symbol;
        Name = name;
        Mint = mint;
    }
}