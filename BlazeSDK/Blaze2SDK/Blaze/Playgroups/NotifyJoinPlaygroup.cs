using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct NotifyJoinPlaygroup
    {
        
        [TdfMember("INFO")]
        public PlaygroupInfo mPlaygroupInfo;
        
        [TdfMember("MLST")]
        public List<PlaygroupMemberInfo> mPlaygroupMemberInfos;
        
        [TdfMember("USER")]
        public uint mJoiningBlazeId;
        
    }
}
