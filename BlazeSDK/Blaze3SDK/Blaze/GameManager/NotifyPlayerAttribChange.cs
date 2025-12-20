using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyPlayerAttribChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mPlayerAttribs;

		[TdfMember("PID")]
		public long mPlayerId;

	}
}
