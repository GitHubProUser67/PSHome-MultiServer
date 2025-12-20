using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct CreateGameStatus
	{

		[TdfMember("EVST")]
		public EvaluateStatus mEvaluateStatus;

		[TdfMember("NOMP")]
		public uint mNumOfMatchedPlayers;

		[TdfMember("MMSN")]
		public uint mNumOfMatchmakingSession;

        [Flags]
        public enum EvaluateStatus
        {
            PlayerCountSufficient = 1,
            AcceptableHostFound = 2,
            TeamSizesSufficient = 4
        }

    }
}
