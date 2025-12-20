using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ListUserEntitlements2Request
	{

		[TdfMember("ETAG")]
		public string mEntitlementTag;

		[TdfMember("TYPE")]
		public EntitlementType mEntitlementType;

		[TdfMember("GDAY")]
		public string mGrantDate;

		[TdfMember("GNLS")]
		public List<string> mGroupNameList;

		[TdfMember("HAUP")]
		public bool mHasAuthorizedPersona;

		[TdfMember("EPSN")]
		public ushort mPageNo;

		[TdfMember("EPSZ")]
		public ushort mPageSize;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PJID")]
		public string mProjectId;

		[TdfMember("RECU")]
		public bool mRecursiveSearch;

		[TdfMember("STAT")]
		public EntitlementStatus mStatus;

		[TdfMember("TERD")]
		public string mTerminationDate;

		[TdfMember("BUID")]
		public long mUserId;

	}
}
