using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GetLegalDocContentRequest
	{

		[TdfMember("TEXT")]
		public ContentType mContentType;

		[TdfMember("CTRY")]
		public string mIsoCountryCode;

		[TdfMember("LANG")]
		public string mIsoLanguage;

		[TdfMember("PTFM")]
		public string mPlatform;

		public enum ContentType : int
		{
			PLAIN = 0,
			HTML = 1,
		}

	}
}
