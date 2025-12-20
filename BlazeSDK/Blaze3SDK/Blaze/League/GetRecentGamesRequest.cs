using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetRecentGamesRequest
	{

		[TdfMember("CONT")]
		public uint mCount;

		[TdfMember("LGID")]
		public uint mLeagueId;

	}
}
