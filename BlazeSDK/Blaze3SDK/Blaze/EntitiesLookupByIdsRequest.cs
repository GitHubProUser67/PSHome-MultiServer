using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct EntitiesLookupByIdsRequest
	{

		[TdfMember("TYPE")]
		public BlazeObjectType mBlazeObjectType;

		[TdfMember("EID")]
		public List<long> mEntityIds;

	}
}
