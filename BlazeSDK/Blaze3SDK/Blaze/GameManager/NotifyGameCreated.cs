using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameCreated
	{

		[TdfMember("GID")]
		public uint mGameId;

	}
}
