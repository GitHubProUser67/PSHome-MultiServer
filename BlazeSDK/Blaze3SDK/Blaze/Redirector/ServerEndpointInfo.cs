using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerEndpointInfo
	{

		[TdfMember("ADRS")]
		public List<ServerAddressInfo> mAddresses;

		[TdfMember("CHAN")]
		public string mChannel;

		[TdfMember("CCON")]
		public uint mCurrentConnections;

		[TdfMember("DEC")]
		public string mDecoder;

		[TdfMember("ENC")]
		public string mEncoder;

		[TdfMember("MCON")]
		public uint mMaxConnections;

		[TdfMember("PROT")]
		public string mProtocol;

	}
}
