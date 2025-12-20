using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct CheckLegalDocRequest
	{

		[TdfMember("TURI")]
		public string mLegalDocUri;

	}
}
