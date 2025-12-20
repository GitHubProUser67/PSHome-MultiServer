using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyHostMigrationStart
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PMIG")]
		public HostMigrationType mMigrationType;

		[TdfMember("HOST")]
		public long mNewHostId;

		[TdfMember("SLOT")]
		public byte mNewHostSlotId;

	}
}
