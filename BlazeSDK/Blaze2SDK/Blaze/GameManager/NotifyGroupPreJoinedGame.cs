using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGroupPreJoinedGame
    {
        
        [TdfMember("GID")]
        public uint mGameId;
        
        [TdfMember("GRID")]
        public ulong mUserGroupId;
        
        [TdfMember("JMET")]
        public JoinMethod mJoinMethod;
        
        [TdfMember("MMID")]
        public uint mMatchmakingSessionId;
        
        [TdfMember("SID")]
        public uint mUserSessionId;
        
        [TdfMember("SLOT")]
        public SlotType mRequestedSlotType;
        
        [TdfMember("TEAM")]
        public ushort mJoiningTeamId;
        
        [TdfMember("TIDX")]
        public ushort mJoiningTeamIndex;
        
    }
}
