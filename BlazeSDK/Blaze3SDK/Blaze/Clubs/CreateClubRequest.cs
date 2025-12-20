using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct CreateClubRequest
	{

		[TdfMember("DMID")]
		public uint mClubDomainId;

		[TdfMember("CSET")]
		public ClubSettings mClubSettings;

		[TdfMember("CNAM")]
		public string mName;

		[TdfMember("CLTG")]
		public List<string> mTagList;

	}
}
