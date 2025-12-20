using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ListMemberInfoUpdate
	{

		[TdfMember("LMID")]
		public ListMemberInfo mListMemberInfo;

		[TdfMember("LUPT")]
		public ListUpdateType mListUpdateType;

	}
}
