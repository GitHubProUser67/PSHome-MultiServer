using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct GetProductAssociationResponse
	{

		[TdfMember("CCDT")]
		public string mCreateDate;

		[TdfMember("CSER")]
		public string mExternalRef;

		[TdfMember("CSFN")]
		public string mFileName;

		[TdfMember("UID")]
		public ulong mId;

		[TdfMember("CMDT")]
		public string mModifiedDate;

		[TdfMember("CSN")]
		public string mName;

		[TdfMember("PLST")]
		public List<ProductAssociation> mProductAssociationList;

		[TdfMember("CPDN")]
		public string mProductName;

		[TdfMember("CPJN")]
		public string mProjectNumber;

		[TdfMember("CSTA")]
		public string mStatus;

		[TdfMember("CST")]
		public string mType;

		[TdfMember("CURI")]
		public string mUri;

	}
}
