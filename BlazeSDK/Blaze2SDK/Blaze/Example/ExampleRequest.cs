using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Example
{
    [TdfStruct]
    public struct ExampleRequest
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// </summary>
        [TdfMember("NMAP")]
        public SortedDictionary<string, Nested> mNestedMap;
        
        [TdfMember("NUM")]
        public int mNum;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 32
        /// </summary>
        [TdfMember("SMAP")]
        public SortedDictionary<string, string> mStringMap;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("TEXT")]
        [StringLength(256)]
        public string mText;
        
    }
}
