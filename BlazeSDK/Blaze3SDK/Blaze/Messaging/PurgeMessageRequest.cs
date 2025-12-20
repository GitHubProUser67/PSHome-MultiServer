using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct PurgeMessageRequest
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

		[TdfMember("TYPE")]
		public uint mType;

	}
}
