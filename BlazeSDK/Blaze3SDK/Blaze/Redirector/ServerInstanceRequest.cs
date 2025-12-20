using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerInstanceRequest
	{

		[TdfMember("BTIM")]
		public string mBlazeSDKBuildDate;

		[TdfMember("BSDK")]
		public string mBlazeSDKVersion;

		[TdfMember("LOC")]
		public uint mClientLocale;

		[TdfMember("CLNT")]
		public string mClientName;

		[TdfMember("CSKU")]
		public string mClientSkuId;

		[TdfMember("CLTP")]
		public ClientType mClientType;

		[TdfMember("CVER")]
		public string mClientVersion;

		[TdfMember("PROF")]
		public string mConnectionProfile;

		[TdfMember("DSDK")]
		public string mDirtySDKVersion;

		[TdfMember("ENV")]
		public string mEnvironment;

		[TdfMember("FPID")]
		public FirstPartyId mFirstPartyId;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PLAT")]
		public string mPlatform;

	}
}
