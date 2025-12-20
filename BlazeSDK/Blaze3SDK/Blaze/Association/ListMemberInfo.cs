using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ListMemberInfo
	{

		[TdfMember("LMID")]
		public ListMemberId mListMemberId;

		[TdfMember("TIME")]
		public long mTimeAdded;

	}
}
