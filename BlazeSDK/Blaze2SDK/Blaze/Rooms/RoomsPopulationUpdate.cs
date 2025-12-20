using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct RoomsPopulationUpdate
    {

        [TdfStruct]
        public struct RoomAttributes
        {

            /// <summary>
            /// Max Key String Length: 32
            /// Max Value String Length: 256
            /// </summary>
            [TdfMember("ATTR")]
            public SortedDictionary<string, string> mAttributeMap;

        }

        [TdfMember("POPA")]
        public SortedDictionary<uint, RoomAttributes> mPopulationAttributes;

        [TdfMember("POPM")]
        public SortedDictionary<uint, uint> mPopulation;

    }
}
