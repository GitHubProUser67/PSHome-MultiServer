using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct LookupRoomDataList
	{

		[TdfMember("RMSL")]
		public List<RoomData> mRoomDataList;

	}
}
