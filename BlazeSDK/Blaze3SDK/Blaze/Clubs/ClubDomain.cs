using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubDomain
	{

		[TdfMember("AMRP")]
		public bool mAllowMemberToRetrievePassword;

		[TdfMember("DMID")]
		public uint mClubDomainId;

		[TdfMember("DNAM")]
		public string mDomainName;

		[TdfMember("DXGM")]
		public ushort mMaxGMsPerClub;

		[TdfMember("DXIA")]
		public ushort mMaxInactiveDaysPerClub;

		[TdfMember("DXIV")]
		public ushort mMaxInvitationsPerUserOrClub;

		[TdfMember("DXMB")]
		public uint mMaxMembersPerClub;

		[TdfMember("DXMS")]
		public ushort mMaxMembershipsPerUser;

		[TdfMember("DXNW")]
		public ushort mMaxNewsItemsPerClub;

		[TdfMember("DUED")]
		public bool mTrackMembershipInUED;

	}
}
