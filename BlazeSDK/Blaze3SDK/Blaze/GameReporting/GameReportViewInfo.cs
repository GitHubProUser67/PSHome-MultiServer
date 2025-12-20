using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GameReportViewInfo
	{

		[TdfMember("DESC")]
		public string mDesc;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("VNAM")]
		public string mName;

		[TdfMember("GTYP")]
		public string mTypeName;

	}
}
