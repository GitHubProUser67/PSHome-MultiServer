using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ListMembers
	{

		[TdfMember("INFO")]
		public ListInfo mInfo;

		[TdfMember("MEML")]
		public List<ListMemberInfo> mListMemberInfoVector;

		[TdfMember("OFRC")]
		public uint mOffset;

		[TdfMember("TOCT")]
		public uint mTotalCount;

	}
}
