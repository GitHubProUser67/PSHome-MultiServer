using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct RoomsPopulationUpdate
	{

		[TdfMember("POPM")]
		public SortedDictionary<uint, uint> mPopulation;

		[TdfMember("POPA")]
		public SortedDictionary<uint, RoomAttributes> mPopulationAttributes;

        [TdfStruct]
        public struct RoomAttributes
        {

            [TdfMember("ATTR")]
            public SortedDictionary<string, string> mAttributeMap;

        }

    }
}
