using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameAttribChange
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mGameAttribs;

		[TdfMember("GID")]
		public uint mGameId;

	}
}
