using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerListResponse
	{

		[TdfMember("LIST")]
		public List<ServerInfoData> mServers;

	}
}
