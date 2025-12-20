using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct GetMemberCountsResponse
	{

		[TdfMember("OCON")]
		public uint mOnlineMemberCount;

		[TdfMember("CONT")]
		public uint mTotalMemberCount;

		[TdfMember("TNID")]
		public uint mTournamentId;

	}
}
