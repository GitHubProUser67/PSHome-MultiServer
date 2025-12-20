using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Example
{
    [TdfStruct]
    public struct ExampleError
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MSG")]
        [StringLength(256)]
        public string mMessage;
        
    }
}
