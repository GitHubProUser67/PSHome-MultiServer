using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct IpAddress
	{

		[TdfMember("IP")]
		public uint mIp;

		[TdfMember("PORT")]
		public ushort mPort;

	}
}
