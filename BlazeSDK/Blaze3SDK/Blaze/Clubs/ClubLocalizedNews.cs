using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubLocalizedNews
	{

		[TdfMember("CLID")]
		public uint mAssociateClubId;

		[TdfMember("NWCC")]
		public long mContentCreator;

		[TdfMember("NWFL")]
		public ClubNewsFlags mFlags;

		[TdfMember("NWID")]
		public BlazeObjectId mNewsId;

		[TdfMember("PERS")]
		public string mPersona;

		[TdfMember("NTXT")]
		public string mText;

		[TdfMember("TMST")]
		public uint mTimestamp;

		[TdfMember("NWTY")]
		public NewsType mType;

	}
}
