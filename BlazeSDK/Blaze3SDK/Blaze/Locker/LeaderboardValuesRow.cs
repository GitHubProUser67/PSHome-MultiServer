using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardValuesRow
	{

		[TdfMember("ATTR")]
		public List<Attribute> mAttrs;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("EID")]
		public long mEntityId;

		[TdfMember("ENAM")]
		public string mEntityName;

		[TdfMember("RANK")]
		public int mRank;

		[TdfMember("TAGS")]
		public List<string> mTags;

	}
}
