namespace Blaze2SDK.Blaze.GameManager
{
    public enum MatchmakingResult : int
    {
        SUCCESS_CREATED_GAME = 0x0,
        SUCCESS_JOINED_NEW_GAME = 0x1,
        SUCCESS_JOINED_EXISTING_GAME = 0x2,
        SESSION_TIMED_OUT = 0x3,
        SESSION_CANCELED = 0x4,
        SESSION_TERMINATED = 0x5,
        SESSION_ERROR_GAME_SETUP_FAILED = 0x6,
    }
}
