using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct SubContentInfo
	{

		[TdfMember("GURL")]
		public string mGetURL;

		[TdfMember("STTS")]
		public Status mStatus;

		[TdfMember("UURL")]
		public string mUploadURL;

		[TdfMember("XID")]
		public string mXrefId;

	}
}
