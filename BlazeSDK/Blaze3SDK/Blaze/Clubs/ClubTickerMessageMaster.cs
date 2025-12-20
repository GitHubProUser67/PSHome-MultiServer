using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubTickerMessageMaster
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("EXUI")]
		public long mExcludeUserId;

		[TdfMember("INUI")]
		public long mIncludeUserId;

		[TdfMember("CTMS")]
		public ClubTickerMessage mMessage;

		[TdfMember("PRMS")]
		public string mParams;

	}
}
