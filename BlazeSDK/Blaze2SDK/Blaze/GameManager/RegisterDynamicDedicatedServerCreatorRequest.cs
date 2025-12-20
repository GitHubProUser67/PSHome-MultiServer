using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct RegisterDynamicDedicatedServerCreatorRequest
    {
        
        /// <summary>
        /// Max Key String Length: 36
        /// </summary>
        [TdfMember("LMAP")]
        public SortedDictionary<string, MachineLoadCapacity> mMachineLoadCapacityMap;
        
    }
}
