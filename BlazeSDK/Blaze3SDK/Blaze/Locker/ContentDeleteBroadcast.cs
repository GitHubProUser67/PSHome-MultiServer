using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ContentDeleteBroadcast
	{

		[TdfMember("DELE")]
		public List<CacheDelete> mCacheDeletes;

	}
}
