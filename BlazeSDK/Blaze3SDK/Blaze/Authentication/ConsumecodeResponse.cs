using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ConsumecodeResponse
	{

		[TdfMember("EXRF")]
		public string mExtRef;

		[TdfMember("KEY")]
		public string mKeyCode;

		[TdfMember("MCNT")]
		public long mMultiUseCount;

		[TdfMember("MFLG")]
		public byte mMultiUseFlag;

		[TdfMember("MLMT")]
		public long mMultiUseLimit;

		[TdfMember("PRID")]
		public string mProductId;

		[TdfMember("PRMN")]
		public string mProductName;

		[TdfMember("STAT")]
		public KeymasterCodeStatus mStatus;

		[TdfMember("UID")]
		public long mUserId;

	}
}
