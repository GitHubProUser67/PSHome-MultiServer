using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatValues
	{

		[TdfMember("AGGR")]
		public List<EntityStatAggregates> mEntityAggrList;

		[TdfMember("STAT")]
		public List<EntityStats> mEntityStatsList;

	}
}
