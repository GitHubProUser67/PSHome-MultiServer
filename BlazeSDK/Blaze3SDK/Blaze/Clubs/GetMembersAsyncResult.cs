using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetMembersAsyncResult
	{

		[TdfMember("MMBR")]
		public ClubMember mClubMember;

		[TdfMember("SQID")]
		public uint mSequenceID;

	}
}
