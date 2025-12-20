using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetRecentGamesResponse
	{

		[TdfMember("RCGM")]
		public List<GameResult> mResults;

	}
}
