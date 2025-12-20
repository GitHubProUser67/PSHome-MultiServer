using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct PostAuthResponse
	{

		[TdfMember("PSS")]
		public PssConfig mPssConfig;

		[TdfMember("TELE")]
		public GetTelemetryServerResponse mTelemetryServer;

		[TdfMember("TICK")]
		public GetTickerServerResponse mTickerServer;

		[TdfMember("UROP")]
		public UserOptions mUserOptions;

	}
}
