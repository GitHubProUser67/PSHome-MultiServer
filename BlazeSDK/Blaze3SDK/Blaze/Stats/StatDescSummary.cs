using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatDescSummary
	{

		[TdfMember("CATG")]
		public string mCategory;

		[TdfMember("DFLT")]
		public string mDefaultValue;

		[TdfMember("DRVD")]
		public bool mDerived;

		[TdfMember("FRMT")]
		public string mFormat;

		[TdfMember("KIND")]
		public string mKind;

		[TdfMember("LDSC")]
		public string mLongDesc;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("SDSC")]
		public string mShortDesc;

		[TdfMember("TYPE")]
		public int mType;

	}
}
