using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyHostMigrationFinished
	{

		[TdfMember("GID")]
		public uint mGameId;

	}
}
