using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatDescs
	{

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("STAT")]
		public List<StatDescSummary> mStatDescs;

	}
}
