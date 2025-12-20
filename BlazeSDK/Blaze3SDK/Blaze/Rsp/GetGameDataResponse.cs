using Blaze3SDK.Blaze.GameManager;
using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct GetGameDataResponse
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mGameAttribs;

		[TdfMember("GNAM")]
		public string mGameName;

		[TdfMember("GSET")]
		public GameSettings mGameSettings;

		[TdfMember("PRES")]
		public PresenceMode mPresenceMode;

	}
}
