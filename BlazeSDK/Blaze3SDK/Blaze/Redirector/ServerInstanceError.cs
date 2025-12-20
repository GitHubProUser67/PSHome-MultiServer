using Tdf;

namespace Blaze3SDK.Blaze.Redirector
{
	[TdfStruct]
	public struct ServerInstanceError
	{

		[TdfMember("MSGS")]
		public List<string> mMessages;

	}
}
