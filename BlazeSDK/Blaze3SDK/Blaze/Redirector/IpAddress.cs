using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct IpAddress
	{

		[TdfMember("HOST")]
		public string mHostname;

		[TdfMember("IP")]
		public uint mIp;

		[TdfMember("PORT")]
		public ushort mPort;

	}
}
