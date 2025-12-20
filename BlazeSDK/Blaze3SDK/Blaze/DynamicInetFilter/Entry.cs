using Tdf;

namespace Blaze3SDK.Blaze.DynamicInetFilter
{
	[TdfStruct(0xC43D1029)]
	public struct Entry
	{

		[TdfMember("COMM")]
		public string mComment;

		[TdfMember("GRP")]
		public string mGroup;

		[TdfMember("OWNR")]
		public string mOwner;

		[TdfMember("RID")]
		public uint mRowId;

		[TdfMember("SNET")]
		public CidrBlock mSubNet;

	}
}
