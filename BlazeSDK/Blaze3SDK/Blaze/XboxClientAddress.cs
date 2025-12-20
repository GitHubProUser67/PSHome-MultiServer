using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct XboxClientAddress
	{

		[TdfMember("XDDR")]
		public byte[] mXnAddr;

		[TdfMember("XUID")]
		public ulong mXuid;

	}
}
