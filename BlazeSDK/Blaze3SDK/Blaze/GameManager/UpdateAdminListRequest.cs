using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct UpdateAdminListRequest
	{

		[TdfMember("PID")]
		public long mAdminPlayerId;

		[TdfMember("GID")]
		public uint mGameId;

	}
}
