using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct CheckLeagueStateRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

	}
}
