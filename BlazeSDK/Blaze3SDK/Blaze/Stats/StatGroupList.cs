using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatGroupList
	{

		[TdfMember("GRPS")]
		public List<StatGroupSummary> mGroups;

	}
}
