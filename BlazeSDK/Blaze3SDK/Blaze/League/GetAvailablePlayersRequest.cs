using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetAvailablePlayersRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

	}
}
