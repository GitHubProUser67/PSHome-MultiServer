using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct HostNameAddress
	{

		[TdfMember("NAME")]
		public string mHostName;

		[TdfMember("PORT")]
		public ushort mPort;

	}
}
