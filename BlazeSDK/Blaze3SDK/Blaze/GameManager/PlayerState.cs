namespace Blaze3SDK.Blaze.GameManager
{
	public enum PlayerState : int
	{
		RESERVED = 0,
		QUEUED = 1,
		ACTIVE_CONNECTING = 2,
		ACTIVE_MIGRATING = 3,
		ACTIVE_CONNECTED = 4,
		ACTIVE_KICK_PENDING = 5,
	}
}
