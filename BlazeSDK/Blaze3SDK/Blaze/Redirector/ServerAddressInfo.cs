using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerAddressInfo
	{

		[TdfMember("ADDR")]
		public ServerAddress mAddress;

		[TdfMember("TYPE")]
		public ServerAddressType mType;

	}
}
