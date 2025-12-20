using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetAvailablePlayersResponse
	{

		[TdfMember("MSLR")]
		public SortedDictionary<uint, AvailablePlayer> mPlayers;

	}
}
