using Solnet.Wallet;

namespace PoolWatcher.Raydium.Config;

/// <summary>
/// Raydium SDK Constants (Fixed values)
/// </summary>
public static class Constants
{
    /// <summary>
    /// The Solana Mainnet Program ID with Raydium Pool
    /// </summary>
    public static readonly ProgramID MAINNET_PROGRAM_ID = new ProgramID(
        serumMarket: new PublicKey("9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin"),
        openbookMarket: new PublicKey("srmqPvymJeFKQ4zGQed1GFppgkRHL9kaELCbyksJtPX"),

        util1216: new PublicKey("CLaimxFqjHzgTJtAGHU47NPhg6qrc5sCnpC4tBLyABQS"),

        farmV3: new PublicKey("EhhTKczWMGQt46ynNeRX1WfeagwwJd7ufHvCDjRxjo5Q"),
        farmV5: new PublicKey("9KEPoZmtHUrBbhWN1v1KWLMkkvwY6WLtAVUCPRtRjP4z"),
        farmV6: new PublicKey("FarmqiPv5eAj3j1GMdMCMUGXqPUvmquZtMy86QH6rzhG"),

        ammV4: new PublicKey("675kPX9MHTjS2zt1qfr1NYHuzeLXfQM9H24wFSUt1Mp8"),
        ammStable: new PublicKey("5quBtoiQqxF9Jv6KYKctB59NT3gtJD2Y65kdnB1Uev3h"),

        clmm: new PublicKey("CAMMCzo5YL8w4VFF8KVHrK22GGUsp5VTaW7grrKgrWqK"),
        router: new PublicKey("routeUGWgWzqBWFcrCfv8tritsqukccJPu3q5GPP3xS")
    );
    
    /// <summary>
    /// Address of the SPL Token program
    /// </summary>
    public static readonly PublicKey TOKEN_PROGRAM_ID = new PublicKey("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");
    
    /// <summary>
    /// Wrapped SOL Token
    /// </summary>
    public static readonly Token WSOL = new Token(
        programID: TOKEN_PROGRAM_ID,
        mint: new PublicKey("So11111111111111111111111111111111111111112"),
        decimals: 9,
        symbol: "WSOL",
        name: "Wrapped SOL"
    );
}