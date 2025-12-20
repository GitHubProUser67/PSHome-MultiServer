using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyCreateDynamicDedicatedServerGame
    {
        
        [TdfMember("GREQ")]
        public CreateGameRequest mCreateGameRequest;
        
        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("MID")]
        [StringLength(36)]
        public string mMachineId;
        
    }
}
