using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetClubRecordbookResponse
	{

		[TdfMember("CLRL")]
		public List<ClubRecord> mClubRecordList;

	}
}
