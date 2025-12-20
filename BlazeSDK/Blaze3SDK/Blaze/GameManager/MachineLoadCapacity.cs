using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct MachineLoadCapacity
	{

		[TdfMember("VSTR")]
		public string mGameProtocolVersionString;

		[TdfMember("CAP")]
		public uint mMaxPlayerCapacity;

		[TdfMember("PSAS")]
		public string mPingSiteAlias;

	}
}
