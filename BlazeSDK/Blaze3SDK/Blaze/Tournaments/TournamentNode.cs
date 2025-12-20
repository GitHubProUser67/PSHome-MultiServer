using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct TournamentNode
	{

		[TdfMember("NOID")]
		public uint mNodeId;

		[TdfMember("ATTO")]
		public string mUserOneAttribute;

		[TdfMember("UIDO")]
		public long mUserOneId;

		[TdfMember("METO")]
		public string mUserOneMetaData;

		[TdfMember("NAMO")]
		public string mUserOneName;

		[TdfMember("SCOO")]
		public uint mUserOneScore;

		[TdfMember("TEAO")]
		public int mUserOneTeam;

		[TdfMember("ATTT")]
		public string mUserTwoAttribute;

		[TdfMember("UIDT")]
		public long mUserTwoId;

		[TdfMember("METT")]
		public string mUserTwoMetaData;

		[TdfMember("NAMT")]
		public string mUserTwoName;

		[TdfMember("SCOT")]
		public uint mUserTwoScore;

		[TdfMember("TEAT")]
		public int mUserTwoTeam;

	}
}
