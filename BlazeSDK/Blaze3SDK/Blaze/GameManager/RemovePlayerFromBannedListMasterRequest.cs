using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct RemovePlayerFromBannedListMasterRequest
	{

		[TdfMember("NID")]
		public long mAccountId;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PID")]
		public long mPlayerId;

	}
}
