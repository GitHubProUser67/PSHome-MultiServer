using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct MachineLoadCapacity
    {
        
        [TdfMember("CAP")]
        public uint mMaxPlayerCapacity;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PSAS")]
        [StringLength(64)]
        public string mPingSiteAlias;
        
    }
}
