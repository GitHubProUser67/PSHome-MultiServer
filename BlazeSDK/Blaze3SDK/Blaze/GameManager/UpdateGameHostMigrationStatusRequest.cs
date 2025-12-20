using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct UpdateGameHostMigrationStatusRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("MTYP")]
		public HostMigrationType mHostMigrationType;

	}
}
