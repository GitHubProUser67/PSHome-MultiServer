using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct MemberInfo
    {
        
        [TdfMember("BLID")]
        public uint mBlazeId;
        
        [TdfMember("ONLN")]
        public bool mIsOnline;
        
    }
}
