using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct ServerMessage
	{

		[TdfMember("FLAG")]
		public ServerFlags mFlags;

		[TdfMember("MGID")]
		public uint mMessageId;

		[TdfMember("PYLD")]
		public ClientMessage mPayload;

		[TdfMember("SRCE")]
		public BlazeObjectId mSource;

		[TdfMember("NAME")]
		public string mSourceName;

		[TdfMember("TIME")]
		public uint mTimestamp;

	}
}
