using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ListIdentification
	{

		[TdfMember("LNM")]
		public string mListName;

		[TdfMember("TYPE")]
		public uint mListType;

	}
}
