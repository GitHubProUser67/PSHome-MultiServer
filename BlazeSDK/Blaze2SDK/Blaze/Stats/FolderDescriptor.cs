using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct FolderDescriptor
    {
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("FLDS")]
        [StringLength(128)]
        public string mDescription;
        
        [TdfMember("FLID")]
        public uint mFolderId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("FLNM")]
        [StringLength(64)]
        public string mName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SDES")]
        [StringLength(32)]
        public string mShortDesc;
        
    }
}
