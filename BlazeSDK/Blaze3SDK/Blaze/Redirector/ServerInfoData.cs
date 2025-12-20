using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct(0xDBFBD5BC)]
	public struct ServerInfoData
	{

		[TdfMember("AMAP")]
		public List<AddressRemapEntry> mAddressRemaps;

		[TdfMember("XMST")]
		public List<ServerInstance> mAuxMasters;

		[TdfMember("XSLV")]
		public List<ServerInstance> mAuxSlaves;

		[TdfMember("LOCN")]
		public string mBuildLocation;

		[TdfMember("BTGT")]
		public string mBuildTarget;

		[TdfMember("BTIM")]
		public string mBuildTime;

		[TdfMember("CVER")]
		public List<string> mCompatibleClientVersions;

		[TdfMember("CGVS")]
		public string mConfigVersion;

		[TdfMember("XDNS")]
		public uint mDefaultDnsAddress;

		[TdfMember("SVID")]
		public uint mDefaultServiceId;

		[TdfMember("DEPO")]
		public string mDepotLocation;

		[TdfMember("IVER")]
		public List<string> mIncompatibleClientVersions;

		[TdfMember("INST")]
		public List<ServerInstance> mInstances;

		[TdfMember("MSTR")]
		public ServerInstance mMasterInstance;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("NMAP")]
		public List<NameRemapEntry> mNameRemaps;

		[TdfMember("NASP")]
		public string mPersonaNamespace;

		[TdfMember("PLAT")]
		public string mPlatform;

		[TdfMember("SNMS")]
		public List<string> mServiceNames;

		[TdfMember("VERS")]
		public string mVersion;

	}
}
