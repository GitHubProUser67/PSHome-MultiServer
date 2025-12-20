using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct SetCategoryAttributesRequest
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mAttributes;

		[TdfMember("CTID")]
		public uint mCategoryId;

	}
}
