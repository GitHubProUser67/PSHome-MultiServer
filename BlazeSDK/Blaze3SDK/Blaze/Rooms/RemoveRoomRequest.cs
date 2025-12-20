using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct RemoveRoomRequest
	{

		[TdfMember("CTID")]
		public uint mCategoryId;

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
