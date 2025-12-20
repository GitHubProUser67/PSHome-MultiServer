using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ListData
	{

		[TdfMember("ID")]
		public uint mId;

		[TdfMember("OFFL")]
		public bool mLoadOfflineUED;

		[TdfMember("SIZE")]
		public uint mMaxSize;

		[TdfMember("MUTA")]
		public bool mMutualAction;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PRID")]
		public uint mPairId;

		[TdfMember("ROLL")]
		public bool mRollover;

		[TdfMember("SCRI")]
		public bool mSubscribe;

	}
}
