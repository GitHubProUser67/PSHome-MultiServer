using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Example
{
    [TdfStruct]
    public struct ExampleResponse
    {
        
        [TdfMember("ENUM")]
        public ExampleResponseEnum mRegularEnum;
        
        [TdfMember("LIST")]
        public List<int> mMyList;
        
        [TdfMember("MAP")]
        public SortedDictionary<int, ExampleRequest> mMyMap;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MSG")]
        [StringLength(256)]
        public string mMessage;
        
        public enum ExampleResponseEnum : int
        {
            EXAMPLE_ENUM_UNKNOWN = 0x0,
            EXAMPLE_ENUM_SUCCESS = 0x1,
            EXAMPLE_ENUM_FAILED = 0x2,
        }
        
    }
}
