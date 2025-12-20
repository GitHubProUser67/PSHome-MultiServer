using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct FindClubs2Request
	{

		[TdfMember("MOSL")]
		public List<MemberOnlineStatus> mMemberOnlineStatusFilter;

		[TdfMember("MOSC")]
		public uint mMemberOnlineStatusSum;

		[TdfMember("RQST")]
		public FindClubsRequest mParams;

	}
}
