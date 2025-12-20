using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct GetTournamentsResponse
	{

		[TdfMember("TRNS")]
		public List<TournamentData> mTournaments;

	}
}
