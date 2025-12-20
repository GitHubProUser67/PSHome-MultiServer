using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct GetGameDataRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

	}
}
