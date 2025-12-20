using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct GetListsRequest
	{

		[TdfMember("ALST")]
		public List<ListInfo> mListInfoVector;

		[TdfMember("MXRC")]
		public uint mMaxResultCount;

		[TdfMember("OFRC")]
		public uint mOffset;

	}
}
