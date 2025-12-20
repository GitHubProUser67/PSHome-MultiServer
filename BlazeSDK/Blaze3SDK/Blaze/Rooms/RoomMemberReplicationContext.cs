using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0xD037FF4E)]
	public struct RoomMemberReplicationContext
	{

		[TdfMember("UPRE")]
		public RoomMemberReplicationReason mReason;

		[TdfMember("USID")]
		public long mUserId;

		public enum RoomMemberReplicationReason : int
		{
			USER_LEFT = 0,
			USER_KICKED = 1,
			USER_JOINED = 2,
			USER_LOGOUT = 3,
			ROOM_CREATED = 4,
			ROOM_DESTROYED = 5,
			ATTRIBUTES_UPDATE = 6,
		}

	}
}
