using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct(0xEB344310)]
	public struct PlaygroupCensusData
	{

		[TdfMember("PIPN")]
		public uint mNumOfPlayersInPlaygroup;

		[TdfMember("PGN")]
		public uint mNumOfPlaygroup;

	}
}
