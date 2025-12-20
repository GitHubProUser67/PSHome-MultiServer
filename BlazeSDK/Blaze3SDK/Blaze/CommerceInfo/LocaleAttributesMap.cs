using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct LocaleAttributesMap
	{

		[TdfMember("LAMP")]
		public SortedDictionary<string, string> mLocaleAttributeMap;

	}
}
