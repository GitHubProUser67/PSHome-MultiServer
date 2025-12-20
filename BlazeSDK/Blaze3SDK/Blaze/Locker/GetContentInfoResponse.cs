using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct GetContentInfoResponse
	{

		[TdfMember("INFO")]
		public ContentInfo mContentInfo;

	}
}
