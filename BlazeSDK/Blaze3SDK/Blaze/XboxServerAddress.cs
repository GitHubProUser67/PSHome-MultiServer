using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct XboxServerAddress
	{

		[TdfMember("PORT")]
		public ushort mPort;

		[TdfMember("SVID")]
		public uint mSid;

		[TdfMember("SITE")]
		public string mSiteName;

	}
}
