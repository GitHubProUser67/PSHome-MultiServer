using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct SetNewsItemHiddenRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("ISHD")]
		public bool mIsHidden;

		[TdfMember("BLID")]
		public BlazeObjectId mNewsId;

	}
}
