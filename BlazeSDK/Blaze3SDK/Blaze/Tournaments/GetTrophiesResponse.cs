using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct GetTrophiesResponse
	{

		[TdfMember("TROP")]
		public List<TournamentTrophyData> mTrophies;

	}
}
