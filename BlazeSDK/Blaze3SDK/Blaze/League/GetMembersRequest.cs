using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetMembersRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("SKIP")]
		public int mSkipGamesRemaining;

	}
}
