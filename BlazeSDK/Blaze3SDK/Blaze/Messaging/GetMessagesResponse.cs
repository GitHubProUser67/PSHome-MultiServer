using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct GetMessagesResponse
	{

		[TdfMember("MSLT")]
		public List<ServerMessage> mMessages;

	}
}
