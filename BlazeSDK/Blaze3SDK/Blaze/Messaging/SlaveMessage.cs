using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct SlaveMessage
	{

		[TdfMember("MESG")]
		public ServerMessage mMessage;

		[TdfMember("TUID")]
		public List<uint> mTargetUserSessionIds;

	}
}
