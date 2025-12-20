using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct StressLoginRequest
	{

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("NUID")]
		public ulong mNucleusId;

		[TdfMember("PNAM")]
		public string mPersonaName;

	}
}
