using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct GetMemberCountsRequest
	{

		[TdfMember("TNID")]
		public uint mTournamentId;

	}
}
