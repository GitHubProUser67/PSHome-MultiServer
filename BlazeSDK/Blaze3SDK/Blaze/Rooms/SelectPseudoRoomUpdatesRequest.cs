using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct SelectPseudoRoomUpdatesRequest
	{

		[TdfMember("CTID")]
		public uint mCategoryId;

		[TdfMember("PVAL")]
		public string mPseudoValue;

	}
}
