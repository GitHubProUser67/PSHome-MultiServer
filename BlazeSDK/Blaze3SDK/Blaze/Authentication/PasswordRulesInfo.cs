using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct PasswordRulesInfo
	{

		[TdfMember("MAXS")]
		public ushort mMaxLength;

		[TdfMember("MINS")]
		public ushort mMinLength;

		[TdfMember("VDCH")]
		public string mValidCharacters;

	}
}
