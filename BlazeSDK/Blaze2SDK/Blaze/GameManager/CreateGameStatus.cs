using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct CreateGameStatus
    {

        [TdfMember("EVST")]
        public EvaluateStatus mEvaluateStatus;

        [TdfMember("MMSN")]
        public uint mNumOfMatchmakingSession;

        [TdfMember("NOMP")]
        public uint mNumOfMatchedPlayers;

        [Flags]
        public enum EvaluateStatus
        {

        }

    }
}
