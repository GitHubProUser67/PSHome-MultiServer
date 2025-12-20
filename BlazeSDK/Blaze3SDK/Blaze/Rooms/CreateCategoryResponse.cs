using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct CreateCategoryResponse
	{

		[TdfMember("CDAT")]
		public RoomCategoryData mCategoryData;

		[TdfMember("MDAT")]
		public RoomMemberData mMemberData;

		[TdfMember("RDAT")]
		public RoomData mRoomData;

	}
}
