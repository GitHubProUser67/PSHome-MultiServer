using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct InstanceRemoveInfo
	{

		[TdfMember("IID")]
		public int mInstanceId;

		[TdfMember("INAM")]
		public string mInstanceName;

		[TdfMember("SNAM")]
		public string mServiceName;

		[TdfMember("SNMS")]
		public List<string> mServiceNames;

		[TdfMember("TYPE")]
		public InstanceType mType;

	}
}
