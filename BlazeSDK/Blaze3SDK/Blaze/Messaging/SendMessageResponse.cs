using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct SendMessageResponse
	{

		[TdfMember("MGID")]
		public uint mMessageId;

		[TdfMember("MIDS")]
		public List<uint> mMessageIds;

	}
}
