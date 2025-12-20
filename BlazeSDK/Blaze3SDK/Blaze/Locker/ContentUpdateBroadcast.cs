using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ContentUpdateBroadcast
	{

		[TdfMember("UPDT")]
		public List<CacheRowUpdate> mCacheUpdates;

	}
}
