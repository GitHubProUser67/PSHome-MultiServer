using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct GetServerDetailsResponse
	{

		[TdfMember("ALST")]
		public List<long> mAdminList;

		[TdfMember("BLST")]
		public List<long> mBanList;

		[TdfMember("MLST")]
		public List<MapRotation> mMapRotations;

		[TdfMember("PLST")]
		public List<Preset> mPresets;

		[TdfMember("SERV")]
		public Server mServer;

		[TdfMember("SETT")]
		public ServerSettings mServerSettings;

		[TdfMember("VLST")]
		public List<long> mVipList;

	}
}
