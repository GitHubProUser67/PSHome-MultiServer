using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Playgroups
{
    [TdfStruct]
    public struct PlaygroupInfo
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mPlaygroupAttributes;
        
        [TdfMember("ENBV")]
        public bool mEnableVoIP;
        
        [TdfMember("HNET")]
        public NetworkAddress mHostNetworkAddress;
        
        [TdfMember("HSID")]
        public byte mHostSlotId;
        
        [TdfMember("JOIN")]
        public PlaygroupJoinability mPlaygroupJoinability;
        
        [TdfMember("MLIM")]
        public ushort mMaxMembers;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mName;
        
        [TdfMember("NTOP")]
        public GameNetworkTopology mNetworkTopology;
        
        [TdfMember("OWNR")]
        public uint mOwnerBlazeId;
        
        [TdfMember("PGID")]
        public uint mId;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("UKEY")]
        [StringLength(32)]
        public string mUniqueKey;
        
        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("UUID")]
        [StringLength(36)]
        public string mUUID;
        
        [TdfMember("VOIP")]
        public VoipTopology mVoipNetwork;
        
        [TdfMember("XNNC")]
        public byte[] mXnetNonce;
        
        [TdfMember("XSES")]
        public byte[] mXnetSession;
        
    }
}
