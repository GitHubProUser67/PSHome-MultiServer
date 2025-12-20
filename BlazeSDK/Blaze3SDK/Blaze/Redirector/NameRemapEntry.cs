using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct NameRemapEntry
	{

		[TdfMember("DPRT")]
		public ushort mDstPort;

		[TdfMember("SIP")]
		public string mHostname;

		[TdfMember("SID")]
		public uint mServiceId;

		[TdfMember("SITE")]
		public string mSiteName;

		[TdfMember("SPRT")]
		public ushort mSrcPort;

	}
}
