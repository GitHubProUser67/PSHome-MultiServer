using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct FetchConfigResponse
	{

		[TdfMember("CONF")]
		public SortedDictionary<string, string> mConfig;

	}
}
