using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportColumnKey
	{

		[TdfMember("ANAM")]
		public string mAttributeName;

		[TdfMember("AIDX")]
		public ushort mIndex;

		[TdfMember("TABN")]
		public string mTable;

	}
}
