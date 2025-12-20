using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct KeyScopes
	{

		[TdfMember("KSIT")]
		public SortedDictionary<string, KeyScopeItem> mKeyScopesMap;

	}
}
