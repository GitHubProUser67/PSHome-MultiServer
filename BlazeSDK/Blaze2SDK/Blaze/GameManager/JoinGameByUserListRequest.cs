using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct JoinGameByUserListRequest
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("ATTR")]
        public SortedDictionary<string, string> mPlayerAttribs;
        
        [TdfMember("GID")]
        public uint mGameId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GVER")]
        [StringLength(64)]
        public string mGameProtocolVersionString;
        
        [TdfMember("JMET")]
        public JoinMethod mJoinMethod;
        
        [TdfMember("PLST")]
        public List<uint> mPlayerIdList;
        
        [TdfMember("PNET")]
        public NetworkAddress mPlayerNetworkAddress;
        
        [TdfMember("SLOT")]
        public SlotType mRequestedSlotType;
        
        [TdfMember("TEAM")]
        public ushort mJoiningTeamId;
        
        [TdfMember("TIDX")]
        public ushort mJoiningTeamIndex;
        
    }
}
