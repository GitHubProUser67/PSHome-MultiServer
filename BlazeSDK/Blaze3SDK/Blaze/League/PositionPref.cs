using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct PositionPref
	{

		[TdfMember("RDPF")]
		public List<sbyte> mPreferences;

	}
}
