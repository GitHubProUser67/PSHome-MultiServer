using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct FindClubsAsyncResult
	{

		[TdfMember("CLUB")]
		public Club mClub;

		[TdfMember("SQID")]
		public uint mSequenceID;

	}
}
