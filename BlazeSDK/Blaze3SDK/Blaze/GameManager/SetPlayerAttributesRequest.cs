using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SetPlayerAttributesRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mPlayerAttributes;

		[TdfMember("PID")]
		public long mPlayerId;

	}
}
