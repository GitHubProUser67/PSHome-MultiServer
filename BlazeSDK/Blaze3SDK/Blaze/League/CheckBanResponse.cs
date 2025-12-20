using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct CheckBanResponse
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("STAT")]
		public uint mState;

		[TdfMember("USER")]
		public long mUserId;

	}
}
