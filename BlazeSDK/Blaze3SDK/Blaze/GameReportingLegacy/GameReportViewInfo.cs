using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
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

	}
}
