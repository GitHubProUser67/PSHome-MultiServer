using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct GetGameReportViewRequest
	{

		[TdfMember("MAXR")]
		public uint mMaxRows;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("QVAR")]
		public List<string> mQueryVarValues;

	}
}
