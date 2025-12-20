using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct LeaderboardValuesRow
    {
        
        [TdfMember("ATTR")]
        public List<Attribute> mAttrs;
        
        [TdfMember("CID")]
        public int mContentId;
        
        [TdfMember("EID")]
        public uint mEntityId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ENAM")]
        [StringLength(64)]
        public string mEntityName;
        
        [TdfMember("RANK")]
        public int mRank;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TAGS")]
        public List<string> mTags;
        
    }
}
