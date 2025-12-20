using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct EntityStatAggregates
	{

		[TdfMember("AGGR")]
		public List<string> mAggrValues;

		[TdfMember("TYPE")]
		public AggregateType mType;

	}
}
