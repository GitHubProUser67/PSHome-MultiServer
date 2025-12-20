using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubNews
	{

		[TdfMember("CLID")]
		public uint mAssociateClubId;

		[TdfMember("NWCC")]
		public long mContentCreator;

		[TdfMember("NPRL")]
		public string mParamList;

		[TdfMember("PERS")]
		public string mPersona;

		[TdfMember("NSIS")]
		public string mStringId;

		[TdfMember("NTXT")]
		public string mText;

		[TdfMember("TMST")]
		public uint mTimestamp;

		[TdfMember("NWTY")]
		public NewsType mType;

	}
}
