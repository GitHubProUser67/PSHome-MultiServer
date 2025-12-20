using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct KeyScopedStatValues
	{

		[TdfMember("GRNM")]
		public string mGroupName;

		[TdfMember("KEY")]
		public string mKeyString;

		[TdfMember("LAST")]
		public bool mLast;

		[TdfMember("STS")]
		public StatValues mStatValues;

		[TdfMember("VID")]
		public uint mViewId;

	}
}
