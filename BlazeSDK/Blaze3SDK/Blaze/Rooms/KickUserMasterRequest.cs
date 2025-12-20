using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct KickUserMasterRequest
	{

		[TdfMember("BANU")]
		public bool mBanUser;

		[TdfMember("RMID")]
		public uint mRoomId;

		[TdfMember("SSID")]
		public uint mSessionId;

		[TdfMember("MBID")]
		public long mUserId;

	}
}
