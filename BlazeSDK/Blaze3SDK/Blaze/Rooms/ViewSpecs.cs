using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct ViewSpecs
	{

		[TdfMember("VMAP")]
		public SortedDictionary<uint, RoomViewData> mSpecMap;

	}
}
