using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct DestroyGameListRequest
	{

		[TdfMember("GLID")]
		public uint mListId;

	}
}
