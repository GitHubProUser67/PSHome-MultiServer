using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GetFullGameDataRequest
	{

		[TdfMember("GIDL")]
		public List<uint> mGameIdList;

		[TdfMember("PIDL")]
		public List<string> mPersistedGameIdList;

	}
}
