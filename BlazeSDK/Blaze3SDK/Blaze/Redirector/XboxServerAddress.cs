using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct XboxServerAddress
	{

		[TdfMember("PORT")]
		public ushort mPort;

		[TdfMember("SID")]
		public uint mServiceId;

		[TdfMember("SITE")]
		public string mSiteName;

	}
}
