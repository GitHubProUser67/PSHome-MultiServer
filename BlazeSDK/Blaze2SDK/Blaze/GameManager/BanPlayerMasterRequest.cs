using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct BanPlayerMasterRequest
    {
        
        [TdfMember("CNTX")]
        public ushort mPlayerRemovedTitleContext;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("NID")]
        public long mNucleusId;
        
        [TdfMember("PID")]
        public uint mBlazeId;
        
    }
}
