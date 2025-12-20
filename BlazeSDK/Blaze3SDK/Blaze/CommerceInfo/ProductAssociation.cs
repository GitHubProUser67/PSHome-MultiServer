using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct ProductAssociation
	{

		[TdfMember("SPDF")]
		public bool mIsDefault;

		[TdfMember("CCAT")]
		public string mSrcCatalog;

		[TdfMember("CPRD")]
		public string mSrcProductId;

	}
}
