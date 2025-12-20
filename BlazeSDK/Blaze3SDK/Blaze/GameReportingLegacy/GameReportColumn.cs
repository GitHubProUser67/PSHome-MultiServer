using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameReportColumn
	{

		[TdfMember("ANAM")]
		public string mAttributeName;

		[TdfMember("ATYP")]
		public string mAttributeType;

		[TdfMember("LDSC")]
		public string mDesc;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("FRMT")]
		public string mFormat;

		[TdfMember("ATID")]
		public uint mIndex;

		[TdfMember("KIND")]
		public string mKind;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("SDSC")]
		public string mShortDesc;

		[TdfMember("DTYP")]
		public int mType;

		[TdfMember("VALU")]
		public List<string> mValues;

	}
}
