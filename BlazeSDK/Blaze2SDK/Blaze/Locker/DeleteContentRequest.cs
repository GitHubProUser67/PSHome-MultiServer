using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct DeleteContentRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CCAT")]
        [StringLength(32)]
        public string mContentCategory;
        
        [TdfMember("CIDS")]
        public List<int> mContentId;
        
    }
}
