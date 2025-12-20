using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubsComponentInfo
	{

		[TdfMember("CLDM")]
		public SortedDictionary<uint, uint> mClubsByDomain;

		[TdfMember("CLCN")]
		public uint mClubsCount;

		[TdfMember("MBDM")]
		public SortedDictionary<uint, uint> mMembersByDomain;

		[TdfMember("MBCN")]
		public uint mMembersCount;

	}
}
