using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct JoinRoomResponse
	{

		[TdfMember("CDAT")]
		public RoomCategoryData mCategoryData;

		[TdfMember("CRIT")]
		public string mFailedCriteria;

		[TdfMember("VERS")]
		public uint mMapVersion;

		[TdfMember("MDAT")]
		public RoomMemberData mMemberData;

		[TdfMember("RDAT")]
		public RoomData mRoomData;

		[TdfMember("VDAT")]
		public RoomViewData mViewData;

	}
}
