using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct LeagueUser
	{

		[TdfMember("BLID")]
		public long mBlazeId;

		[TdfMember("PERS")]
		public string mPersona;

	}
}
