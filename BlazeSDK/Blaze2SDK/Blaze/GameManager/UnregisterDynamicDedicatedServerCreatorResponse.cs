using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct UnregisterDynamicDedicatedServerCreatorResponse
    {
        
        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("MLST")]
        public List<string> mMachineIdList;
        
    }
}
