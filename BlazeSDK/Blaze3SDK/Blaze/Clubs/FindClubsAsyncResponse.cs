using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct FindClubsAsyncResponse
	{

		[TdfMember("CONT")]
		public uint mCount;

		[TdfMember("SQID")]
		public uint mSequenceID;

		[TdfMember("CTCT")]
		public uint mTotalCount;

	}
}
