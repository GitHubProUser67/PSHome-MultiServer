using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct ReturnDedicatedServerToPoolRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

	}
}
