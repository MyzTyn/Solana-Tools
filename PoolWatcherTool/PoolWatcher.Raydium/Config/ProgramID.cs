using Solnet.Wallet;

namespace PoolWatcher.Raydium.Config;

/// <summary>
/// Raydium SDK Program ID
/// </summary>
public struct ProgramID
{
    public readonly PublicKey SERUM_MARKET;
    public readonly PublicKey OPENBOOK_MARKET;

    public readonly PublicKey UTIL1216;

    public readonly PublicKey FarmV3;
    public readonly PublicKey FarmV5;
    public readonly PublicKey FarmV6;

    public readonly PublicKey AmmV4;
    public readonly PublicKey AmmStable;

    public readonly PublicKey CLMM;
    public readonly PublicKey Router;

    // Constructor to initialize all fields
    public ProgramID(PublicKey serumMarket, PublicKey openbookMarket, PublicKey util1216, PublicKey farmV3, PublicKey farmV5, PublicKey farmV6, PublicKey ammV4, PublicKey ammStable, PublicKey clmm, PublicKey router)
    {
        SERUM_MARKET = serumMarket;
        OPENBOOK_MARKET = openbookMarket;
        UTIL1216 = util1216;
        FarmV3 = farmV3;
        FarmV5 = farmV5;
        FarmV6 = farmV6;
        AmmV4 = ammV4;
        AmmStable = ammStable;
        CLMM = clmm;
        Router = router;
    }
}