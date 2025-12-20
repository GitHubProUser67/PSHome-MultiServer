using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct LeaveGameByGroupMasterRequest
    {
        
        [TdfMember("CNTX")]
        public ushort mPlayerRemovedTitleContext;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("SIDL")]
        public List<uint> mSessionIdList;
        
    }
}
