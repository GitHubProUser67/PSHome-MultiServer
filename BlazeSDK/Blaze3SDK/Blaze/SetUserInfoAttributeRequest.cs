using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct SetUserInfoAttributeRequest
	{

		[TdfMember("ATTV")]
		public ulong mAttributeBits;

		[TdfMember("ULST")]
		public List<BlazeObjectId> mBlazeObjectIdList;

		[TdfMember("MASK")]
		public ulong mMaskBits;

	}
}
