using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerInstanceInfo
	{

		[TdfMember("ADDR")]
		public ServerAddress mAddress;

		[TdfMember("AMAP")]
		public List<AddressRemapEntry> mAddressRemaps;

		[TdfMember("XDNS")]
		public uint mDefaultDnsAddress;

		[TdfMember("MSGS")]
		public List<string> mMessages;

		[TdfMember("NMAP")]
		public List<NameRemapEntry> mNameRemaps;

		[TdfMember("SECU")]
		public bool mSecure;

	}
}
