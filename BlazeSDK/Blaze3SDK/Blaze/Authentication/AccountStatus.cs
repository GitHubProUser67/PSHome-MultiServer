namespace Blaze3SDK.Blaze.Authentication
{
	public enum AccountStatus : int
	{
		UNKNOWN = 0,
		ACTIVE = 1,
		BANNED = 2,
		CHILD_APPROVED = 3,
		CHILD_PENDING = 4,
		DEACTIVATED = 5,
		DELETED = 6,
		DISABLED = 7,
		PENDING = 8,
		TENTATIVE = 9,
		VOLATILE = 10,
	}
}
