using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct GetMyTournamentDetailsResponse
	{

		[TdfMember("ACTI")]
		public bool mIsActive;

		[TdfMember("LEVL")]
		public uint mLevel;

		[TdfMember("TEAM")]
		public int mTeam;

		[TdfMember("ATTR")]
		public string mTournAttribute;

		[TdfMember("TID")]
		public uint mTournamentId;

		[TdfMember("TREE")]
		public List<TournamentNode> mTournamentTree;

	}
}
