using Tdf;

namespace Blaze3SDK.Blaze.GpsContentController
{
	[TdfStruct]
	public struct FetchContentResponse
	{

		[TdfMember("ANVP")]
		public SortedDictionary<string, string> mAttributeMap;

		[TdfMember("EURL")]
		public string mExternalURL;

	}
}
