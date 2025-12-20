using Blaze2SDK.Blaze.CommerceInfo;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct AddPointsToWalletResponse
    {

        [TdfMember("WBL")]
        public WalletBalance mWalletBalance;

    }
}
