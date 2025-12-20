using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct FetchMessageRequest
	{

		[TdfMember("FLAG")]
		public MatchFlags mFlags;

		[TdfMember("MGID")]
		public uint mMessageId;

		[TdfMember("SORT")]
		public MessageOrder mOrderBy;

		[TdfMember("PIDX")]
		public uint mPageIndex;

		[TdfMember("PSIZ")]
		public uint mPageSize;

		[TdfMember("SRCE")]
		public BlazeObjectId mSource;

		[TdfMember("STAT")]
		public uint mStatus;

		[TdfMember("SMSK")]
		public uint mStatusMask;

		[TdfMember("TARG")]
		public BlazeObjectId mTarget;

		[TdfMember("TYPE")]
		public uint mType;

		[TdfMember("TYPL")]
		public List<uint> mTypeList;

	}
}
