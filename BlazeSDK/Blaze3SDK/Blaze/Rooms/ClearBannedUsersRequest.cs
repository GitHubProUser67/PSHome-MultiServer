using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct ClearBannedUsersRequest
	{

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
