using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct CountMessagesRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("MSTY")]
		public MessageType mMessageType;

	}
}
