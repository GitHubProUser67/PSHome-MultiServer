using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetLegalDocContentResponse
	{

		[TdfMember("TCOT")]
		public string mLegalDocContent;

		[TdfMember("TCOL")]
		public uint mLegalDocContentLength;

		[TdfMember("LDVC")]
		public string mLegalDocVersion;

	}
}
