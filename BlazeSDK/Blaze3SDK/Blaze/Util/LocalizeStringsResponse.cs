using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct LocalizeStringsResponse
	{

		[TdfMember("SMAP")]
		public SortedDictionary<string, string> mLocalizedStrings;

	}
}
