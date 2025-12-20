using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct ViewSpecs
    {
        
        [TdfMember("VMAP")]
        public SortedDictionary<uint, RoomViewData> mSpecMap;
        
    }
}
