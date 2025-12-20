namespace Blaze2SDK.Blaze.GameManager
{
    public enum GameDestructionReason : int
    {
        SYS_GAME_ENDING = 0x0,
        SYS_CREATION_FAILED = 0x1,
        HOST_LEAVING = 0x2,
        TITLE_REASON_BASE_GAME_DESTRUCTION_REASON = 0x3,
    }
}
