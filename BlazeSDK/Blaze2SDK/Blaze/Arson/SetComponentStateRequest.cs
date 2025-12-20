using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct SetComponentStateRequest
    {
        
        [TdfMember("ACTN")]
        public Action mAction;
        
        [TdfMember("ERR")]
        public uint mErrorCode;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mComponentName;
        
        public enum Action : int
        {
            ENABLE = 0x0,
            DISABLE = 0x1,
        }
        
    }
}
