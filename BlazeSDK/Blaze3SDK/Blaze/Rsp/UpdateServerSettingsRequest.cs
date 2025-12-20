using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateServerSettingsRequest
	{

		[TdfMember("SID")]
		public uint mServerId;

		[TdfMember("SETT")]
		public ServerSettings mServerSettings;

	}
}
