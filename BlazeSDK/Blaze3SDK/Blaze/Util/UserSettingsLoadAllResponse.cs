using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct UserSettingsLoadAllResponse
	{

		[TdfMember("SMAP")]
		public SortedDictionary<string, string> mDataMap;

	}
}
