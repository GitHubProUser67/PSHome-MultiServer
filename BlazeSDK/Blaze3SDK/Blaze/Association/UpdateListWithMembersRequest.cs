using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct UpdateListWithMembersRequest
	{

		[TdfMember("LID")]
		public ListIdentification mListIdentification;

		[TdfMember("BIDL")]
		public List<ListMemberInfoUpdate> mListMemberInfoVector;

	}
}
