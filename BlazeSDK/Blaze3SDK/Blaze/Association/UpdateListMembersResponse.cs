using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct UpdateListMembersResponse
	{

		[TdfMember("LMID")]
		public List<ListMemberInfo> mListMemberInfoVector;

		[TdfMember("REM")]
		public List<ListMemberId> mRemovedListMemberIdVector;

	}
}
