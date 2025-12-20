using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct QosPingSiteInfo
	{

		[TdfMember("PSA")]
		public string mAddress;

		[TdfMember("PSP")]
		public ushort mPort;

		[TdfMember("SNA")]
		public string mSiteName;

	}
}
