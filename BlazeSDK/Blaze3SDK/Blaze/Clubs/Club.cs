using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct(0xAFD76551)]
	public struct Club
	{

		[TdfMember("DMID")]
		public uint mClubDomainId;

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("CLIN")]
		public ClubInfo mClubInfo;

		[TdfMember("CLST")]
		public ClubSettings mClubSettings;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("CLTG")]
		public List<string> mTagList;

	}
}
