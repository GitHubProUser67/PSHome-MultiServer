using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GetGameReportQuery
	{

		[TdfMember("QNAM")]
		public string mName;

	}
}
