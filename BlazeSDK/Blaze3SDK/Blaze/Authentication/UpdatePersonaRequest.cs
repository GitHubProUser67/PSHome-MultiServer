using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct UpdatePersonaRequest
	{

		[TdfMember("DSNM")]
		public string mDisplayName;

		[TdfMember("PID")]
		public long mPersonaId;

	}
}
