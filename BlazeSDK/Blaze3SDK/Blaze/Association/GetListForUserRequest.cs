using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct GetListForUserRequest
	{

		[TdfMember("BID")]
		public long mBlazeId;

		[TdfMember("LID")]
		public ListIdentification mListIdentification;

		[TdfMember("MXRC")]
		public uint mMaxResultCount;

		[TdfMember("OFRC")]
		public uint mOffset;

	}
}
