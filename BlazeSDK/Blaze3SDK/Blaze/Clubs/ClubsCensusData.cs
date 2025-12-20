using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct(0xDE7B5AB1)]
	public struct ClubsCensusData
	{

		[TdfMember("TCM")]
		public uint mNumOfClubMembers;

		[TdfMember("MBD")]
		public SortedDictionary<uint, uint> mNumOfClubMembersByDomain;

		[TdfMember("TNC")]
		public uint mNumOfClubs;

		[TdfMember("CBD")]
		public SortedDictionary<uint, uint> mNumOfClubsByDomain;

		[TdfMember("OCM")]
		public uint mNumOfOnlineClubMembers;

		[TdfMember("OMD")]
		public SortedDictionary<uint, uint> mNumOfOnlineClubMembersByDomain;

		[TdfMember("TOC")]
		public uint mNumOfOnlineClubs;

		[TdfMember("OCD")]
		public SortedDictionary<uint, uint> mNumOfOnlineClubsByDomain;

	}
}
