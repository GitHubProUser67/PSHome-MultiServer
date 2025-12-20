using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct SetDraftProfileRequest
	{

		[TdfMember("PROF")]
		public DraftProfile mDraftProfile;

		[TdfMember("LGID")]
		public uint mLeagueId;

	}
}
