using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct LeaderboardTreeNode
    {
        
        [TdfMember("CHDE")]
        public uint mNext2Last;
        
        [TdfMember("CHDS")]
        public uint mFirstChild;
        
        [TdfMember("LAST")]
        public bool mLastNode;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mNodeName;
        
        [TdfMember("NDID")]
        public uint mNodeId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("RTNM")]
        [StringLength(64)]
        public string mRootName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SDES")]
        [StringLength(32)]
        public string mShortDesc;
        
    }
}
