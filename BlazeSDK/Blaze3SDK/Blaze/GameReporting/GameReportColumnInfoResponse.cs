using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportColumnInfoResponse
	{

		[TdfMember("CIL")]
		public List<GameReportColumnInfo> mColumnInfoList;

	}
}
