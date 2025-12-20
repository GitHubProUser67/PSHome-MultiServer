using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct JoinTournamentRequest
	{

		[TdfMember("TNID")]
		public uint mId;

		[TdfMember("TEAM")]
		public int mTeam;

		[TdfMember("ATTR")]
		public string mTournAttribute;

	}
}
