using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardViewColumn
	{

		[TdfMember("DISP")]
		public int mDisplay;

		[TdfMember("FRMT")]
		public string mFormat;

		[TdfMember("LDSC")]
		public string mLongDesc;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("SDSC")]
		public string mShortDesc;

		[TdfMember("TYPE")]
		public string mType;

	}
}
