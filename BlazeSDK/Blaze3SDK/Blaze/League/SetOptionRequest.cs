using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct SetOptionRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("GMID")]
		public long mMemberId;

		[TdfMember("OPID")]
		public uint mOptionId;

		[TdfMember("VALU")]
		public uint mValue;

	}
}
