using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct RoomRemoved
	{

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
