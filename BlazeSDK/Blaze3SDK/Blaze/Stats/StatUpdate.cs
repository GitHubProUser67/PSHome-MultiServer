using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatUpdate
	{

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("TYPE")]
		public int mUpdateType;

		[TdfMember("VALU")]
		public string mValue;

	}
}
