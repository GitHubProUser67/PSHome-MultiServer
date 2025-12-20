using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ConfigLists
	{

		[TdfMember("LSDT")]
		public List<ListData> mListsInfo;

	}
}
