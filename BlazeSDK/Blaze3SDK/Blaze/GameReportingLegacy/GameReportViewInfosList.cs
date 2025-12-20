using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameReportViewInfosList
	{

		[TdfMember("GRPS")]
		public List<GameReportViewInfo> mViewInfo;

	}
}
