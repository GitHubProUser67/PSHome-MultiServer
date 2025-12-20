using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct XboxId
	{

		[TdfMember("GTAG")]
		public string mGamertag;

		[TdfMember("XUID")]
		public ulong mXuid;

	}
}
