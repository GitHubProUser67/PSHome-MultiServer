using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct FindLeaguesResponse
	{

		[TdfMember("LLST")]
		public List<League> mLeagues;

	}
}
