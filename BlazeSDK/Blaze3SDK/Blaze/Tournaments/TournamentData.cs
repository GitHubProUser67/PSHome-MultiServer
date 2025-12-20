using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct(0x9567DCBB)]
	public struct TournamentData
	{

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("TNID")]
		public uint mId;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("RNDS")]
		public uint mNumRounds;

		[TdfMember("NOMM")]
		public uint mOnlineMemberCount;

		[TdfMember("NMEM")]
		public uint mTotalMemberCount;

		[TdfMember("TMET")]
		public string mTrophyMetaData;

		[TdfMember("TNAM")]
		public string mTrophyName;

	}
}
