using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerInstance
	{

		[TdfMember("CLTP")]
		public List<ClientType> mClientTypes;

		[TdfMember("CWD")]
		public string mCurrentWorkingDirectory;

		[TdfMember("ENDP")]
		public List<ServerEndpointInfo> mEndpoints;

		[TdfMember("SVC")]
		public bool mInService;

		[TdfMember("ID")]
		public int mInstanceId;

		[TdfMember("NAME")]
		public string mInstanceName;

		[TdfMember("LOAD")]
		public int mLoad;

	}
}
