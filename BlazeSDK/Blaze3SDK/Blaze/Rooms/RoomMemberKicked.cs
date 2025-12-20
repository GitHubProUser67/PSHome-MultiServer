using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct RoomMemberKicked
	{

		[TdfMember("RMID")]
		public uint mRoomId;

		[TdfMember("MBID")]
		public long mUserId;

	}
}
