using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct ListBannedUserRequest
	{

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
