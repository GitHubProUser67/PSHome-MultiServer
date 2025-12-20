using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubMembership
	{

		[TdfMember("DMID")]
		public uint mClubDomainId;

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("MBER")]
		public ClubMember mClubMember;

		[TdfMember("NAME")]
		public string mClubName;

	}
}
