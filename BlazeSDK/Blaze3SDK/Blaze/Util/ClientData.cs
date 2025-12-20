using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct ClientData
	{

		[TdfMember("TYPE")]
		public ClientType mClientType;

		[TdfMember("IITO")]
		public bool mIgnoreInactivityTimeout;

		[TdfMember("LANG")]
		public uint mLocale;

		[TdfMember("SVCN")]
		public string mServiceName;

	}
}
