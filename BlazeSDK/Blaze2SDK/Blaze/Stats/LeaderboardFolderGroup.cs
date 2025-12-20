using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct LeaderboardFolderGroup
    {
        
        [TdfMember("FLDS")]
        public List<FolderDescriptor> mFolderDescriptors;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("META")]
        [StringLength(128)]
        public string mMetadata;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("OWDS")]
        [StringLength(128)]
        public string mDescription;
        
        [TdfMember("OWID")]
        public uint mFolderId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("OWNM")]
        [StringLength(64)]
        public string mName;
        
        [TdfMember("PRID")]
        public uint mParentId;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SDES")]
        [StringLength(32)]
        public string mShortDesc;
        
    }
}
