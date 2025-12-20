namespace Blaze3SDK.Blaze.GameManager
{
	public enum GameState : int
	{
		NEW_STATE = 0,
		INITIALIZING = 1,
		VIRTUAL = 2,
		PRE_GAME = 130,
		IN_GAME = 131,
		POST_GAME = 4,
		MIGRATING = 5,
		DESTRUCTING = 6,
		RESETABLE = 7,
		REPLAY_SETUP = 8,
	}
}
