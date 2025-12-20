using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GameReportType
	{

		[TdfMember("ATYP")]
		public List<string> mAttributeTypes;

		[TdfMember("GTID")]
		public uint mGameTypeId;

		[TdfMember("GTNA")]
		public string mGameTypeName;

	}
}
