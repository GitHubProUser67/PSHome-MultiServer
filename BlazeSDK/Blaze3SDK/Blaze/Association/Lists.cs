using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct Lists
	{

		[TdfMember("LMAP")]
		public List<ListMembers> mListMembersVector;

	}
}
