using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct LookupUsersRequest
	{

		[TdfMember("LTYP")]
		public LookupType mLookupType;

		[TdfMember("ULST")]
		public List<UserIdentification> mUserIdentificationList;

		public enum LookupType : int
		{
			BLAZE_ID = 0,
			PERSONA_NAME = 1,
			EXTERNAL_ID = 2,
			ACCOUNT_ID = 3,
		}

	}
}
