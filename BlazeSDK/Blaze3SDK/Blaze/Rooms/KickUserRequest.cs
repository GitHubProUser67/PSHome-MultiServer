using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct KickUserRequest
	{

		[TdfMember("BANU")]
		public bool mBanUser;

		[TdfMember("RMID")]
		public uint mRoomId;

		[TdfMember("MBID")]
		public long mUserId;

	}
}
