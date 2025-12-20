using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct TableData
	{

		[TdfMember("COLS")]
		public List<string> mColumns;

		[TdfMember("PKEY")]
		public List<string> mPrimaryKey;

		[TdfMember("TABN")]
		public string mTable;

	}
}
