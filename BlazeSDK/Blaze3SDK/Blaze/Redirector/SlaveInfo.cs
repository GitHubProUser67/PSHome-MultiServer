using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct SlaveInfo
	{

		[TdfMember("INST")]
		public ServerInstance mInstance;

		[TdfMember("NAME")]
		public string mServiceName;

		[TdfMember("SNMS")]
		public List<string> mServiceNames;

		[TdfMember("TYPE")]
		public InstanceType mType;

	}
}
