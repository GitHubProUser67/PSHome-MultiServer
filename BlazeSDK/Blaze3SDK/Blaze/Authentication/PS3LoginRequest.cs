using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct PS3LoginRequest
	{

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("TCKT")]
		public byte[] mPS3Ticket;

	}
}
