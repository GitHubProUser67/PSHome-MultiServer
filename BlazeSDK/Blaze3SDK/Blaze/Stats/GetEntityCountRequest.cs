using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct GetEntityCountRequest
	{

		[TdfMember("CAT")]
		public string mCategory;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("PTYP")]
		public int mPeriodType;

	}
}
