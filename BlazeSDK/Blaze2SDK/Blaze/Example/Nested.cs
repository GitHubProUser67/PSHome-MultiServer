using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Example
{
    [TdfStruct]
    public struct Nested
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 32
        /// </summary>
        [TdfMember("NMPA")]
        public SortedDictionary<string, string> mStringMap;
        
        [TdfMember("NUM")]
        public int mNum;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("TEXT")]
        [StringLength(256)]
        public string mText;
        
    }
}
