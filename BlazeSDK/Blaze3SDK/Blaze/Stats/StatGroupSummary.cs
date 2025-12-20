using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatGroupSummary
	{

		[TdfMember("DESC")]
		public string mDesc;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("NAME")]
		public string mName;

	}
}
