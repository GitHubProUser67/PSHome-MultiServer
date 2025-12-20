using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct ListMemberId
    {
        
        [TdfMember("BLID")]
        public uint mBlazeId;
        
        [TdfMember("ETID")]
        public ExternalMemberId mExternalMemId;
        
    }
}
