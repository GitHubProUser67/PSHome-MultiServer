using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct KeyScopeChangeRequest
	{

		[TdfMember("EID")]
		public long mEntityId;

		[TdfMember("ETP")]
		public BlazeObjectType mEntityType;

		[TdfMember("KSNM")]
		public string mKeyScopeName;

		[TdfMember("KSNV")]
		public long mNewKeyScopeValue;

		[TdfMember("KSOV")]
		public long mOldKeyScopeValue;

	}
}
