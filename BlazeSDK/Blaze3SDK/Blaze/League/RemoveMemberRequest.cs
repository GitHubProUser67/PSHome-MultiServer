using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct RemoveMemberRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("MMBR")]
		public long mMemberId;

	}
}
