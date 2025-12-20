using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct JoinGameByGroupMasterRequest
    {
        
        [TdfMember("BTPL")]
        public ulong mGroupId;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("JMET")]
        public JoinMethod mJoinMethod;
        
        [TdfMember("SIDL")]
        public List<uint> mSessionIdList;
        
        [TdfMember("SLOT")]
        public SlotType mRequestedSlotType;
        
        [TdfMember("TEAM")]
        public ushort mJoiningTeamId;
        
        [TdfMember("TIDX")]
        public ushort mJoiningTeamIndex;
        
    }
}
