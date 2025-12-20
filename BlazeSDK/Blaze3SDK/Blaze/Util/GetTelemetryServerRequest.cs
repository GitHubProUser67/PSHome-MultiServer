using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct GetTelemetryServerRequest
	{

		[TdfMember("CMAC")]
		public string mMacAddress;

		[TdfMember("SNAM")]
		public string mServiceName;

	}
}
