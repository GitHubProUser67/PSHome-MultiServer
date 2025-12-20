using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportViewInfosList
	{

		[TdfMember("INFO")]
		public List<GameReportViewInfo> mViewInfo;

	}
}
