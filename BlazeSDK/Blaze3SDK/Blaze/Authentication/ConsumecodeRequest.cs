using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ConsumecodeRequest
	{

		[TdfMember("KEY")]
		public string mCode;

		[TdfMember("DEID")]
		public uint mDeviceId;

		[TdfMember("GNAM")]
		public string mGroupName;

		[TdfMember("PNID")]
		public bool mIsBindPersona;

		[TdfMember("CDKY")]
		public bool mIsCdKey;

		[TdfMember("PID")]
		public string mProductId;

	}
}
