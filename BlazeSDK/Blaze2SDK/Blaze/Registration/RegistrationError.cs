using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationError
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("MSG")]
        [StringLength(256)]
        public string mMessage;
        
    }
}
