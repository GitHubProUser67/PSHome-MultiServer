using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ListInfo
	{

		[TdfMember("BOID")]
		public BlazeObjectId mBlazeObjId;

		[TdfMember("LID")]
		public ListIdentification mId;

		[TdfMember("LMS")]
		public uint mMaxSize;

		[TdfMember("PRID")]
		public uint mPairId;

		[TdfMember("FLGS")]
		public ListStatusFlags mStatusFlags;

	}
}
