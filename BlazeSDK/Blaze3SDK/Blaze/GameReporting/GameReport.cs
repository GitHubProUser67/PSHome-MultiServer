using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReport
	{

		[TdfMember("GRID")]
		public ulong mGameReportingId;

		[TdfMember("GTYP")]
		public string mGameTypeName;

		[TdfMember("GAME")]
		public object? mReport;

	}
}
