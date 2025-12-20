using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct UnbanUserRequest
	{

		[TdfMember("RMID")]
		public uint mRoomId;

		[TdfMember("USID")]
		public long mUserId;

	}
}
