using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct DynamicConfig
	{

		[TdfMember("AMAX")]
		public uint mMessageAttributeLimit;

	}
}
