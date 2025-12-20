using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerListRequest
	{

		[TdfMember("PROF")]
		public string mConnectionProfile;

		[TdfMember("CNT")]
		public uint mCount;

		[TdfMember("ENV")]
		public string mEnvironment;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PLAT")]
		public string mPlatform;

	}
}
