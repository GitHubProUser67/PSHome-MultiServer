using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct UpdateListMembersRequest
	{

		[TdfMember("LID")]
		public ListIdentification mListIdentification;

		[TdfMember("BIDL")]
		public List<ListMemberId> mListMemberIdVector;

		[TdfMember("MUTA")]
		public bool mMutualAction;

		[TdfMember("VALA")]
		public bool mValidateAdd;

		[TdfMember("VALD")]
		public bool mValidateDelete;

	}
}
