using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct RemoveCategoryRequest
	{

		[TdfMember("CTID")]
		public uint mCategoryId;

	}
}
