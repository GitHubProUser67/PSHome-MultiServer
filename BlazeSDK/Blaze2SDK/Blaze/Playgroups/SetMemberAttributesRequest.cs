using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct SetMemberAttributesRequest
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mMemberAttributes;
        
        [TdfMember("EID")]
        public uint mBlazeId;
        
        [TdfMember("PGID")]
        public uint mPlaygroupId;
        
    }
}
