using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubMessage
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("CLNM")]
		public string mClubName;

		[TdfMember("INID")]
		public uint mMessageId;

		[TdfMember("INVT")]
		public MessageType mMessageType;

		[TdfMember("RECV")]
		public long mReceiverId;

		[TdfMember("RPRS")]
		public string mReceiverPersona;

		[TdfMember("SEND")]
		public long mSenderId;

		[TdfMember("SPRS")]
		public string mSenderPersona;

		[TdfMember("TIME")]
		public uint mTimeSent;

	}
}
