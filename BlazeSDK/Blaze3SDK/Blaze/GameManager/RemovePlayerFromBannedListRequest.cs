using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct RemovePlayerFromBannedListRequest
	{

		[TdfMember("PID")]
		public long mBlazeId;

		[TdfMember("GID")]
		public uint mGameId;

	}
}
