using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct FetchClientConfigRequest
	{

		[TdfMember("CFID")]
		public string mConfigSection;

	}
}
