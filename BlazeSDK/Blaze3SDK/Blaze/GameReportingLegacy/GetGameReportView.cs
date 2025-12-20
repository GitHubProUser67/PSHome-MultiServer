using Tdf;

namespace Blaze3SDK.Blaze.GameReportingLegacy
{
	[TdfStruct]
	public struct GetGameReportView
	{

		[TdfMember("MAXR")]
		public uint mMaxRows;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("QVAR")]
		public List<string> mQueryVarValues;

	}
}
