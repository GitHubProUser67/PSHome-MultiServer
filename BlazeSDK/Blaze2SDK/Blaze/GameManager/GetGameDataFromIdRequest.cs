using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GetGameDataFromIdRequest
    {
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("DNAM")]
        [StringLength(16)]
        public string mListConfigName;
        
        [TdfMember("GLST")]
        public List<uint> mGameIds;
        
    }
}
