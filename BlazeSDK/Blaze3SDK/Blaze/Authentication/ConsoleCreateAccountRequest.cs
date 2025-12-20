using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ConsoleCreateAccountRequest
	{

		[TdfMember("CREQ")]
		public CreateAccountParameters mCreateAccountParameters;

		[TdfMember("XREF")]
		public ulong mExtId;

		[TdfMember("PERS")]
		public string mExtName;

		[TdfMember("TICK")]
		public byte[] mTicketBlob;

		[TdfMember("UID")]
		public long mUserId;

	}
}
