using Tdf;

namespace Blaze2SDK.Blaze.CommerceInfo
{
    [TdfStruct]
    public struct PricePoints
    {
        
        [TdfMember("PPL")]
        public List<PricePoint> mPricePointVector;
        
        [TdfMember("TCTE")]
        public bool mIsFree;
        
    }
}
