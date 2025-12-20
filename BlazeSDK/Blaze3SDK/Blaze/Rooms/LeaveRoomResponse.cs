using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct LeaveRoomResponse
	{

		[TdfMember("CTID")]
		public uint mCategoryId;

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
