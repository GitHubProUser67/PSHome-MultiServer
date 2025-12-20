using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct GetTelemetryServerResponse
	{

		[TdfMember("ADRS")]
		public string mAddress;

		[TdfMember("DISA")]
		public string mDisable;

		[TdfMember("FILT")]
		public string mFilter;

		[TdfMember("ANON")]
		public bool mIsAnonymous;

		[TdfMember("SKEY")]
		public string mKey;

		[TdfMember("LOC")]
		public uint mLocale;

		[TdfMember("NOOK")]
		public string mNoToggleOk;

		[TdfMember("PORT")]
		public uint mPort;

		[TdfMember("SDLY")]
		public uint mSendDelay;

		[TdfMember("SPCT")]
		public uint mSendPercentage;

		[TdfMember("SESS")]
		public string mSessionID;

		[TdfMember("STIM")]
		public string mUseServerTime;

	}
}
