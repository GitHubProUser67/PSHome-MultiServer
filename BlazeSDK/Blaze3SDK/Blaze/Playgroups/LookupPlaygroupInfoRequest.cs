using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct LookupPlaygroupInfoRequest
	{

		[TdfMember("PLST")]
		public List<uint> mPlaygroupIdList;

	}
}
