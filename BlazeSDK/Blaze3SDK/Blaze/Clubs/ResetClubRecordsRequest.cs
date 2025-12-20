using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ResetClubRecordsRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("RCID")]
		public List<uint> mRecordIdList;

	}
}
