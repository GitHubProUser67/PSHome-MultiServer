using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct JoinTournamentResponse
	{

		[TdfMember("TDAT")]
		public TournamentData mTournament;

	}
}
