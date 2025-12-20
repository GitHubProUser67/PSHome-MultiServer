using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct CheckBanRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("USER")]
		public long mUserId;

	}
}
