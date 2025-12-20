using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct GetListForUser
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ALNM")]
        [StringLength(32)]
        public string mListName;
        
        [TdfMember("BID")]
        public uint mBlazeId;
        
    }
}
