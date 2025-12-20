using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct LookupPlaygroupInfoList
	{

		[TdfMember("PGPS")]
		public List<PlaygroupInfo> mPlaygroupInfoList;

	}
}
