using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0x8B387072)]
	public struct RoomCategoryReplicationContext
	{

		[TdfMember("UPRE")]
		public RoomCategoryUpdateReason mUpdateReason;

		[TdfMember("USID")]
		public long mUserId;

		[TdfMember("SEID")]
		public uint mUserSessionId;

		public enum RoomCategoryUpdateReason : int
		{
			CONFIG_RELOADED = 0,
			USER_CREATED = 1,
			ROOM_EXPANSION_CHANGE = 2,
			ATTRIBUTES_UPDATE = 3,
		}

	}
}
