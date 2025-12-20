namespace Blaze3SDK.Blaze.GameManager
{
	public enum GameDestructionReason : int
	{
		SYS_GAME_ENDING = 0,
		SYS_CREATION_FAILED = 1,
		HOST_LEAVING = 2,
		HOST_INJECTION = 3,
		HOST_EJECTION = 4,
		LOCAL_PLAYER_LEAVING = 5,
		TITLE_REASON_BASE_GAME_DESTRUCTION_REASON = 6,
	}
}
