using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SetGameAttributesRequest
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mGameAttributes;

		[TdfMember("GID")]
		public uint mGameId;

	}
}
