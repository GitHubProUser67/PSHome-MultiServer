using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyXboxSessionInfo
	{

		[TdfMember("PGID")]
		public uint mId;

		[TdfMember("PRES")]
		public bool mUsesPresence;

		[TdfMember("XNNC")]
		public byte[] mXnetNonce;

		[TdfMember("XSES")]
		public byte[] mXnetSession;

	}
}
