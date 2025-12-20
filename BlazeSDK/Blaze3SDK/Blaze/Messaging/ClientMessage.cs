using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct ClientMessage
	{

		[TdfMember("ATTR")]
		public SortedDictionary<uint, string> mAttrMap;

		[TdfMember("FLAG")]
		public MessageFlags mFlags;

		[TdfMember("STAT")]
		public uint mStatus;

		[TdfMember("TAG")]
		public uint mTag;

		[TdfMember("TARG")]
		public BlazeObjectId mTarget;

		[TdfMember("TYPE")]
		public uint mType;

	}
}
