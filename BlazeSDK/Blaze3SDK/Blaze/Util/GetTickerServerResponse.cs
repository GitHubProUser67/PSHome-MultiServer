using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct GetTickerServerResponse
	{

		[TdfMember("ADRS")]
		public string mAddress;

		[TdfMember("SKEY")]
		public string mKey;

		[TdfMember("PORT")]
		public uint mPort;

	}
}
