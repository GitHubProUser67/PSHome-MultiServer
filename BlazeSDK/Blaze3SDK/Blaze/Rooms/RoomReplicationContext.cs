using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0x1D7C1BD4)]
	public struct RoomReplicationContext
	{

		[TdfMember("UPRE")]
		public RoomUpdateReason mUpdateReason;

		[TdfMember("USID")]
		public long mUserId;

		[TdfMember("SEID")]
		public uint mUserSessionId;

		[TdfMember("VWID")]
		public uint mViewId;

		public enum RoomUpdateReason : int
		{
			DELETED_EXPANDED_ROOM = 0,
			DELETED_PSEUDO_ROOM = 1,
			DELETED_EMPTY_USER_CREATED = 2,
			DELETED_BY_USER = 3,
			CONFIG_RELOADED = 4,
			HOST_TRANSFER = 5,
			POPULATION_CHANGED = 6,
			USER_CREATED = 7,
			ATTRIBUTES_UPDATE = 8,
			BAN_LIST_MODIFIED = 9,
		}

	}
}
