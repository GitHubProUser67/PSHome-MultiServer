using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct EntityStats
	{

		[TdfMember("EID")]
		public long mEntityId;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("STAT")]
		public List<string> mStatValues;

	}
}
