using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ConsoleAssociateAccountRequest
	{

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("XREF")]
		public ulong mExtId;

		[TdfMember("PERS")]
		public string mExtName;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("TICK")]
		public byte[] mTicketBlob;

	}
}
