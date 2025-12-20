using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ContentInfos
	{

		[TdfMember("LKRS")]
		public List<ContentInfo> mContentInfoList;

	}
}
