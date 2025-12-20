using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct LookupUsersByPrefixRequest
	{

		[TdfMember("CAP")]
		public uint mMaxResultCount;

		[TdfMember("PREF")]
		public string mPrefixName;

	}
}
