using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct PersonaDetails
	{

		[TdfMember("DSNM")]
		public string mDisplayName;

		[TdfMember("XREF")]
		public ulong mExtId;

		[TdfMember("XTYP")]
		public ExternalRefType mExtType;

		[TdfMember("LAST")]
		public uint mLastAuthenticated;

		[TdfMember("PID")]
		public long mPersonaId;

		[TdfMember("STAS")]
		public PersonaStatus mStatus;

	}
}
