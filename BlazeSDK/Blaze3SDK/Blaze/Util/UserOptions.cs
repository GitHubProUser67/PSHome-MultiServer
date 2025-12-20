using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct UserOptions
	{

		[TdfMember("TMOP")]
		public TelemetryOpt mTelemetryOpt;

		[TdfMember("UID")]
		public long mUserId;

	}
}
