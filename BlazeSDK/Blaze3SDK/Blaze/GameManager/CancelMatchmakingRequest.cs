using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct CancelMatchmakingRequest
	{

		[TdfMember("MSID")]
		public uint mMatchmakingSessionId;

	}
}
