using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameReportingIdChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GRID")]
		public ulong mGameReportingId;

	}
}
