using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct SendSourceMessageRequest
	{

		[TdfMember("PYLD")]
		public ClientMessage mPayload;

		[TdfMember("SRCE")]
		public BlazeObjectId mSource;

	}
}
