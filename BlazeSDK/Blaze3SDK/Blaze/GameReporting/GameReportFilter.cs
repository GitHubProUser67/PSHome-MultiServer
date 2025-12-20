using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportFilter
	{

		[TdfMember("ANAM")]
		public string mAttributeName;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("EXPR")]
		public string mExpression;

		[TdfMember("HVAR")]
		public bool mHasVariable;

		[TdfMember("AIDX")]
		public ushort mIndex;

		[TdfMember("TABN")]
		public string mTable;

	}
}
