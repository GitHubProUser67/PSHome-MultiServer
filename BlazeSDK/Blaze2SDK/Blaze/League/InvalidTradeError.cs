using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct InvalidTradeError
    {
        
        [TdfMember("RESN")]
        public int mReasonCode;
        
    }
}
