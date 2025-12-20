using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct(0xF654DDCF)]
	public struct UserManagerCensusData
	{

		[TdfMember("CPCM")]
		public SortedDictionary<ClientType, uint> mConnectedPlayerCounts;

	}
}
