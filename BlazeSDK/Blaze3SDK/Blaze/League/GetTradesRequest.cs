using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetTradesRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("FORM")]
		public long mMemberId;

	}
}
