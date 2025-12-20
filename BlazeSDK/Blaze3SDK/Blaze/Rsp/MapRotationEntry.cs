using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct MapRotationEntry
	{

		[TdfMember("MODE")]
		public string mGameMode;

		[TdfMember("MAP")]
		public string mMap;

	}
}
