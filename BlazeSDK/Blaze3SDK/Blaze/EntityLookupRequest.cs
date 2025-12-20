using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct EntityLookupRequest
	{

		[TdfMember("NAME")]
		public string mEntityName;

		[TdfMember("TYPE")]
		public string mEntityTypeName;

	}
}
