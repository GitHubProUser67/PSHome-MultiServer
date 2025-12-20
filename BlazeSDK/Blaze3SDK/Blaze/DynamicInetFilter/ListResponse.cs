using Tdf;

namespace Blaze3SDK.Blaze.DynamicInetFilter
{
	[TdfStruct]
	public struct ListResponse
	{

		[TdfMember("ENTS")]
		public List<Entry> mEntries;

	}
}
