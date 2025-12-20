using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct RecordSettings
	{

		[TdfMember("RCID")]
		public uint mRecordId;

		[TdfMember("RCNA")]
		public string mRecordName;

	}
}
