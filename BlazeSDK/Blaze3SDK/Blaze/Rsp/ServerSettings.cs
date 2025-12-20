using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct ServerSettings
	{

		[TdfMember("BID")]
		public int mBannerId;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("MID")]
		public byte mMapRotationId;

		[TdfMember("MESS")]
		public string mMessage;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("SALT")]
		public int mPasswordSalt;

		[TdfMember("PID")]
		public byte mPresetId;

		[TdfMember("TYPE")]
		public ServerType mServerType;

	}
}
