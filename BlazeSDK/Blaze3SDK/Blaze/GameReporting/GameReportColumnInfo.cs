using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportColumnInfo
	{

		[TdfMember("LDSC")]
		public string mDesc;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("FRMT")]
		public string mFormat;

		[TdfMember("CKEY")]
		public GameReportColumnKey mKey;

		[TdfMember("KIND")]
		public string mKind;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("SDSC")]
		public string mShortDesc;

		[TdfMember("DTYP")]
		public int mType;

		[TdfMember("UNKV")]
		public string mUnknownValue;

	}
}
