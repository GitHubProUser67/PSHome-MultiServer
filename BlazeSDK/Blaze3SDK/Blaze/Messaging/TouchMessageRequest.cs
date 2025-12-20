using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct TouchMessageRequest
	{

		[TdfMember("FLAG")]
		public MatchFlags mFlags;

		[TdfMember("MGID")]
		public uint mMessageId;

		[TdfMember("SRCE")]
		public BlazeObjectId mSource;

		[TdfMember("STAT")]
		public uint mStatus;

		[TdfMember("SMSK")]
		public uint mStatusMask;

		[TdfMember("TARG")]
		public BlazeObjectId mTarget;

		[TdfMember("TSTA")]
		public uint mTouchStatus;

		[TdfMember("TMSK")]
		public uint mTouchStatusMask;

		[TdfMember("TYPE")]
		public uint mType;

	}
}
