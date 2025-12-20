using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct Attribute
	{

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("VALU")]
		public string mValue;

	}
}
