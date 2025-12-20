using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetPlayoffSeriesRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

	}
}
