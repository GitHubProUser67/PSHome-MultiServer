using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetClubTickerMessagesResponse
	{

		[TdfMember("MSLI")]
		public List<ClubTickerMessage> mMsgList;

	}
}
