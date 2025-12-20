using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct JoinPlaygroupRequest
    {
        
        [TdfMember("PGID")]
        public uint mId;
        
        [TdfMember("PNET")]
        public NetworkAddress mNetworkAddress;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("UKEY")]
        [StringLength(32)]
        public string mUniqueKey;
        
        [TdfMember("USER")]
        public UserIdentification mUser;
        
    }
}
