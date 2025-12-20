using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetMemberRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("MMBR")]
		public long mMemberId;

	}
}
