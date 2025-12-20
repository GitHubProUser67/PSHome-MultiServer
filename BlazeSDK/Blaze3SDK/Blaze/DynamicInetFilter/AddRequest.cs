using Tdf;

namespace Blaze3SDK.Blaze.DynamicInetFilter
{
	[TdfStruct]
	public struct AddRequest
	{

		[TdfMember("COMM")]
		public string mComment;

		[TdfMember("GRP")]
		public string mGroup;

		[TdfMember("OWNR")]
		public string mOwner;

		[TdfMember("SNET")]
		public CidrBlock mSubNet;

	}
}
