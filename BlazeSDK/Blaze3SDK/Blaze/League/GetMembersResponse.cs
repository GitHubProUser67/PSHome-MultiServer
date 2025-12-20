using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetMembersResponse
	{

		[TdfMember("MBIF")]
		public List<MemberInfo> mMemberInfo;

	}
}
