using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameReset
	{

		[TdfMember("DATA")]
		public ReplicatedGameData mGameData;

	}
}
