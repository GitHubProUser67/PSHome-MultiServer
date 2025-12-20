using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct UpdateClubSettingsRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("CLST")]
		public ClubSettings mClubSettings;

		[TdfMember("CLTG")]
		public List<string> mTagList;

	}
}
