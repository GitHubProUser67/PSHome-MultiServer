using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyAdminListChange
	{

		[TdfMember("ALST")]
		public long mAdminPlayerId;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("OPER")]
		public UpdateAdminListOperation mOperation;

		[TdfMember("UID")]
		public long mUpdaterPlayerId;

	}
}
