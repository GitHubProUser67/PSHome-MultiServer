using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetMembersAsyncResponse
	{

		[TdfMember("CONT")]
		public uint mCount;

		[TdfMember("SQID")]
		public uint mSequenceID;

		[TdfMember("TCON")]
		public uint mTotalCount;

	}
}
