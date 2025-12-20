using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportQueriesList
	{

		[TdfMember("QUER")]
		public List<GameReportQuery> mQueries;

	}
}
