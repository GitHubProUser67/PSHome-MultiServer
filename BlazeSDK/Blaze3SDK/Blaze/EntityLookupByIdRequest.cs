using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct EntityLookupByIdRequest
	{

		[TdfMember("TYPE")]
		public BlazeObjectType mBlazeObjectType;

		[TdfMember("EID")]
		public long mEntityId;

	}
}
