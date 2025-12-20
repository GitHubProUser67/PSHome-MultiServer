using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetDraftProfileRequest
	{

		[TdfMember("CNT")]
		public ushort mCount;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("STRT")]
		public ushort mStartingRank;

	}
}
