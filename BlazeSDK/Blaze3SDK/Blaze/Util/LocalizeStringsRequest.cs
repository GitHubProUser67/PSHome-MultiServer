using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct LocalizeStringsRequest
	{

		[TdfMember("LANG")]
		public uint mLocale;

		[TdfMember("LSID")]
		public List<string> mStringIds;

	}
}
