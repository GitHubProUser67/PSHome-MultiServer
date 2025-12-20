using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct PersonaInfo
	{

		[TdfMember("DTCR")]
		public string mDateCreated;

		[TdfMember("DSNM")]
		public string mDisplayName;

		[TdfMember("LADT")]
		public uint mLastAuthenticated;

		[TdfMember("NSNM")]
		public string mNameSpaceName;

		[TdfMember("PID")]
		public long mPersonaId;

		[TdfMember("STAS")]
		public PersonaStatus mStatus;

		[TdfMember("STRC")]
		public StatusReason mStatusReasonCode;

	}
}
