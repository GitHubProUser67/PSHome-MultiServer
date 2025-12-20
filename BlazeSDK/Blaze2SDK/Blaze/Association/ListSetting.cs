using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct ListSetting
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ALNM")]
        [StringLength(32)]
        public string mListName;
        
        [TdfMember("SUBF")]
        public bool mSubscribe;
        
    }
}
