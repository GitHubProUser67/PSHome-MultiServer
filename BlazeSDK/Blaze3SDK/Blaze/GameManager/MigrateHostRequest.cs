using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct MigrateHostRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("HOST")]
		public long mNewHostPlayer;

	}
}
