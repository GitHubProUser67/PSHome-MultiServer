using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct ScheduledCategorySpec
	{

		[TdfMember("CATS")]
		public RoomCategoryData mCategoryData;

		[TdfMember("DURA")]
		public uint mDuration;

		[TdfMember("RECU")]
		public uint mRecurrence;

		[TdfMember("STAR")]
		public uint mStart;

	}
}
