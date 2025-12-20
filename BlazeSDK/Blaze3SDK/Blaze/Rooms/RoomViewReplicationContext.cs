using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0x5A3762D9)]
	public struct RoomViewReplicationContext
	{

		[TdfMember("UPRE")]
		public RoomViewUpdateReason mUpdateReason;

		public enum RoomViewUpdateReason : int
		{
			CONFIG_RELOADED = 0,
			USER_ROOM_CREATED = 1,
			USER_ROOM_DESTROYED = 2,
		}

	}
}
